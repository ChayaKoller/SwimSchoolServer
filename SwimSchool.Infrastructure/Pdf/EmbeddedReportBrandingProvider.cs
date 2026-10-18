using SwimSchool.Application.Reports.Pdf;

namespace SwimSchool.Infrastructure.Pdf;

public sealed class EmbeddedReportBrandingProvider : IReportBrandingProvider
{
    private readonly Lazy<ReportBranding> branding = new(() => new ReportBranding(
        RightLogo: EmbeddedAssets.Read("logo-gili-galim.png"),
        LeftLogo: EmbeddedAssets.Read("logo-aquarium.jpeg")));

    // Same logos for every teacher today
    public ReportBranding GetFor(int teacherId) => branding.Value;
}
