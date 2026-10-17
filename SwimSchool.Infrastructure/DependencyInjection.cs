using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwimSchool.Application.Auth;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Enrollments;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Lessons;
using SwimSchool.Application.Reports;
using SwimSchool.Application.Students;
using SwimSchool.Application.Teachers;
using SwimSchool.Infrastructure.Authentication;
using SwimSchool.Infrastructure.Persistence;
using SwimSchool.Infrastructure.Persistence.Repositories;
using SwimSchool.Infrastructure.Time;

namespace SwimSchool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string 'ConnectionStrings:Default'");

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<SwimSchoolDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.EnableRetryOnFailure()));

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddHealthChecks().AddDbContextCheck<SwimSchoolDbContext>("database");

        return services;
    }
}
