namespace SwimSchool.API.Options;

public sealed class ClientCorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "Client";

    public string[] AllowedOrigins { get; set; } = [];

    public static ClientCorsOptions Load(IConfiguration configuration, IHostEnvironment environment)
    {
        var options = configuration.GetSection(SectionName).Get<ClientCorsOptions>() ?? new ClientCorsOptions();

        if (!environment.IsDevelopment())
        {
            if (options.AllowedOrigins.Length == 0)
                throw new InvalidOperationException("Cors:AllowedOrigins must contain at least one origin outside Development");

            var invalid = options.AllowedOrigins
                .Where(o => !Uri.TryCreate(o, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                .ToArray();

            if (invalid.Length > 0)
                throw new InvalidOperationException(
                    $"Cors:AllowedOrigins must contain only absolute https URIs. Invalid: {string.Join(", ", invalid)}");
        }

        return options;
    }
}
