using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SwimSchool.Application.Reports.Pdf;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Infrastructure.Pdf;

// The client's treatment report ("דוח טיפולים – נספח לקבלה"). General form: used for every payer without a form of its own.
public sealed class TreatmentReportPdfGenerator(IReportBrandingProvider brandingProvider) : IReportPdfGenerator
{
    private static readonly CultureInfo Hebrew = CultureInfo.GetCultureInfo("he-IL");
    private const string FontFamily = "Heebo";
    private const string TableHeaderColor = "#C6D9F1";

    public HealthFund? Fund => null;

    public byte[] Generate(ReportPdfData data)
    {
        QuestPdfSetup.EnsureConfigured();
        var branding = brandingProvider.GetFor(data.TeacherId);

        return Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.4f, Unit.Centimetre);
            page.ContentFromRightToLeft();
            page.DefaultTextStyle(t => t.FontFamily(FontFamily).FontSize(12));

            page.Header().Element(c => Header(c, branding));
            page.Content().PaddingTop(10).Element(c => Body(c, data));
        })).GeneratePdf();
    }

    // Both logos and a double line
    private static void Header(IContainer container, ReportBranding branding) => container.Column(col =>
    {
        col.Item().Row(row =>
        {
            row.RelativeItem().AlignRight().Height(48).Image(branding.RightLogo).FitHeight();
            row.RelativeItem().AlignLeft().Height(48).Image(branding.LeftLogo).FitHeight();
        });
        col.Item().PaddingTop(6).LineHorizontal(2.5f);
        col.Item().PaddingTop(1.5f).LineHorizontal(0.8f);
    });

    private static void Body(IContainer container, ReportPdfData d) => container.Column(col =>
    {
        col.Spacing(4);
        col.Item().Row(row =>
        {
            row.RelativeItem().Text("בס\"ד").Bold();
            row.RelativeItem().AlignLeft().Text(t => { t.Span("תאריך: ").Bold(); t.Span(Day(d.IssuedOn)); });
        });

        col.Item().PaddingTop(6).AlignCenter().Text("דוח טיפולים – נספח לקבלה").Bold().FontSize(15).Underline();
        col.Item().Text(t => { t.Span("עבור:").Bold().Underline(); t.Span(" " + d.ServiceDescription); });

        col.Item().PaddingTop(4).Text("פרטי החבר מקבל השירות:").Bold().Underline();
        col.Item().Text(t => { t.Span("שם: ").Bold(); t.Span(d.StudentName); });
        col.Item().Text(t => { t.Span("ת.ז: ").Bold(); t.Span(d.StudentIdentityNumber); });

        col.Item().PaddingTop(4).Text("פירוט השירות:").Bold().Underline();
        col.Item().Text(t => { t.Span("מספר טיפולים: ").Bold(); t.Span(d.TreatmentDates.Count.ToString(Hebrew)); });
        col.Item().Text(t => { t.Span(PriceLabel(d.TreatmentType)).Bold(); t.Span(Money(d.PricePerTreatment)); });
        col.Item().Text(t => { t.Span("סה\"כ: ").Bold(); t.Span(Money(d.TotalAmount)); });

        col.Item().PaddingTop(4).Text("פירוט תאריכי הטיפולים:").Bold().Underline();
        col.Item().Element(c => DatesTable(c, d));

        col.Item().PaddingTop(8).AlignLeft().Width(150).Column(s =>
        {
            s.Item().AlignCenter().Text("בברכה").Bold();
            s.Item().AlignCenter().Text(d.Signature).Bold();
        });
    });

    // Two side-by-side blocks like the original form: the first half on the right, the rest on the left.
    // A report has at most 34 dates (validated), so the table always fits on the first page.
    private static void DatesTable(IContainer container, ReportPdfData d)
    {
        var count = d.TreatmentDates.Count;
        var rows = (count + 1) / 2;
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                for (var block = 0; block < 2; block++)
                {
                    c.ConstantColumn(55); c.RelativeColumn(); c.ConstantColumn(70);
                }
            });
            table.Header(h =>
            {
                for (var block = 0; block < 2; block++)
                    foreach (var title in new[] { "מספר טיפול", "תאריך", "סך" })
                        h.Cell().Element(HeaderCell).Text(title).Bold();
            });
            for (var r = 0; r < rows; r++)
                for (var block = 0; block < 2; block++)
                {
                    var i = r + block * rows;
                    var has = i < count;
                    table.Cell().Element(Cell).Text(has ? (i + 1).ToString(Hebrew) : "").Bold();
                    table.Cell().Element(Cell).AlignCenter().Text(has ? Day(d.TreatmentDates[i]) : "");
                    table.Cell().Element(Cell).AlignCenter().Text(has ? Money(d.PricePerTreatment) : "");
                }
        });

        static IContainer HeaderCell(IContainer c) => c.Border(0.7f).Background(TableHeaderColor).PaddingVertical(2).PaddingHorizontal(4).DefaultTextStyle(t => t.FontSize(11));
        static IContainer Cell(IContainer c) => c.Border(0.7f).PaddingVertical(1).PaddingHorizontal(4).DefaultTextStyle(t => t.FontSize(11));
    }

    private static string PriceLabel(TreatmentType type) =>
        type == TreatmentType.Individual ? "עלות טיפול פרטני: " : "עלות טיפול בקבוצה: ";

    private static string Money(decimal value) => value.ToString("#,0.##", Hebrew) + " ₪";

    private static string Day(DateOnly date) => date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
}
