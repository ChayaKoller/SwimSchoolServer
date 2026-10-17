using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using NLog.Web;
using SwimSchool.API.Development;
using SwimSchool.API.Middleware;
using SwimSchool.API.Options;
using SwimSchool.API.Validation;
using SwimSchool.Application;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Infrastructure;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;
var nlogFile = environmentName == Environments.Development ? "nlog.Development.config" : "nlog.config";
var logger = LogManager.Setup().LoadConfigurationFromFile(Path.Combine(AppContext.BaseDirectory, nlogFile)).GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers()
        .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
        .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ValidationResponseFactory.Create);
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var cors = ClientCorsOptions.Load(builder.Configuration, builder.Environment);
    builder.Services.AddCors(o => o.AddPolicy(ClientCorsOptions.PolicyName, p => p
        .WithOrigins(cors.AllowedOrigins)
        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
        .WithHeaders("Content-Type", "Authorization", CorrelationIdMiddleware.HeaderName)
        .WithExposedHeaders(CorrelationIdMiddleware.HeaderName, "Content-Disposition")));

    builder.Services.Configure<ForwardedHeadersOptions>(o =>
    {
        o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        o.KnownNetworks.Clear();
        o.KnownProxies.Clear();
    });

    // TEMPORARY: removed in Stage 9
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.Configure<DevUserOptions>(builder.Configuration.GetSection("DevUser"));
        builder.Services.AddScoped<ICurrentUser, DevCurrentUser>();
    }

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.Services.GetRequiredService<AutoMapper.IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
        await app.Services.InitializeDatabaseAsync();

        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseForwardedHeaders();
        app.UseHsts();
    }

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseCors(ClientCorsOptions.PolicyName);
    // Stage 9: app.UseAuthentication(); app.UseAuthorization();
    app.MapHealthChecks("/health");
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    logger.Fatal(ex, "SwimSchool API failed to start");
    Environment.ExitCode = 1;
}
finally
{
    LogManager.Shutdown();
}
