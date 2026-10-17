using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Infrastructure.Persistence;
using SwimSchool.Infrastructure.Persistence.Seeding;

namespace SwimSchool.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SwimSchoolDbContext>();

        await db.Database.MigrateAsync(cancellationToken);
        await DevelopmentDataSeeder.SeedAsync(db, cancellationToken);

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await DevelopmentDataSeeder.SetDevPasswordsAsync(db, hasher, configuration["DevSeed:Password"], cancellationToken);
    }
}
