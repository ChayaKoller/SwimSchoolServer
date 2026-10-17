using System.Text.Json.Serialization;
using SwimSchool.API.Development;
using SwimSchool.API.Middleware;
using SwimSchool.API.Validation;
using SwimSchool.Application;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'Default'"));

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ValidationResponseFactory.Create);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TEMPORARY: removed in Stage 9
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<DevUserOptions>(builder.Configuration.GetSection("DevUser"));
    builder.Services.AddScoped<ICurrentUser, DevCurrentUser>();
}

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.Services.GetRequiredService<AutoMapper.IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
    await app.Services.InitializeDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Stage 9: app.UseAuthentication(); app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();
