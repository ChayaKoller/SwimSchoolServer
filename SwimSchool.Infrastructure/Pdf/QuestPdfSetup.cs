using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace SwimSchool.Infrastructure.Pdf;

public static class QuestPdfSetup
{
    private static readonly Lazy<bool> Configured = new(() =>
    {
        // Community license: free for organisations under USD 1M annual gross revenue.
        QuestPDF.Settings.License = LicenseType.Community;

        foreach (var font in new[] { "Heebo-Regular.ttf", "Heebo-Bold.ttf" })
            FontManager.RegisterFontFromStream(new MemoryStream(EmbeddedAssets.Read(font)));

        return true;
    });

    public static void EnsureConfigured() => _ = Configured.Value;
}
