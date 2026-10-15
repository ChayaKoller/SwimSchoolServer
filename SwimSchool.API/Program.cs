using System.Text.Json.Serialization;
using SwimSchool.API.Development;
using SwimSchool.Application;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'Default'"));

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
