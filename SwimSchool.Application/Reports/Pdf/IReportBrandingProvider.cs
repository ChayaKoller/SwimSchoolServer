namespace SwimSchool.Application.Reports.Pdf;

public interface IReportBrandingProvider
{
    // Same logos for every teacher today; teacherId is here so per-teacher logos need only a new implementation.
    ReportBranding GetFor(int teacherId);
}
