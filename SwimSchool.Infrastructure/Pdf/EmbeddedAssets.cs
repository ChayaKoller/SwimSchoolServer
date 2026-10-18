namespace SwimSchool.Infrastructure.Pdf;

internal static class EmbeddedAssets
{
    public static byte[] Read(string fileName)
    {
        var assembly = typeof(EmbeddedAssets).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith(".Pdf.Assets." + fileName, StringComparison.Ordinal));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}
