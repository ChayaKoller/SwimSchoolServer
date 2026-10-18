using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports.Pdf;

// Everything the PDF prints, already prepared. The generator never touches the database.
public sealed record ReportPdfData(
    int TeacherId,                       // whose branding (logos) to use
    DateOnly IssuedOn,                   // Israel date
    string ServiceDescription, string StudentName, string StudentIdentityNumber,
    TreatmentType TreatmentType, decimal PricePerTreatment, decimal TotalAmount,
    IReadOnlyList<DateOnly> TreatmentDates,
    string Signature);                   // may contain line breaks
