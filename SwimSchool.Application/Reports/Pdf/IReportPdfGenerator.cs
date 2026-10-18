using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports.Pdf;

public interface IReportPdfGenerator
{
    HealthFund? Fund { get; }            // the fund this form is for; null = the general form
    byte[] Generate(ReportPdfData data);
}
