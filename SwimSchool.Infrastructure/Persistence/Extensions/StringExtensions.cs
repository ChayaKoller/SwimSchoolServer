namespace SwimSchool.Infrastructure.Persistence.Extensions;

public static class StringExtensions
{
    // Escapes LIKE/ILIKE wildcards using "\" as the escape character
    public static string EscapeLikePattern(this string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}
