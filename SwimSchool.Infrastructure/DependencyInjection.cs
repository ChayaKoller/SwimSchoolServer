using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Students;
using SwimSchool.Infrastructure.Persistence;
using SwimSchool.Infrastructure.Persistence.Repositories;

namespace SwimSchool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SwimSchoolDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.EnableRetryOnFailure()));

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
