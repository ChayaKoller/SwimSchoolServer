using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using SwimSchool.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace SwimSchool.Infrastructure.Tests.TestSupport;

// One temporary PostgreSQL 18 for the whole test run, built from our real migrations.
// Docker Desktop must be running. The container is deleted at the end; the dev database is never touched.
public sealed class PostgresDatabase : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public string ConnectionString => _container!.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder().WithImage("postgres:18").Build();
            await _container.StartAsync();
        }
        catch (DockerUnavailableException ex)
        {
            throw new InvalidOperationException("Docker is not running. Start Docker Desktop and run the tests again.", ex);
        }

        await using var db = CreateContext();
        await db.Database.MigrateAsync();
    }

    public Task DisposeAsync() => _container?.DisposeAsync().AsTask() ?? Task.CompletedTask;

    // A separate context = a separate user (for example the teacher on one computer, the secretary on another)
    public SwimSchoolDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<SwimSchoolDbContext>().UseNpgsql(ConnectionString).Options);
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresDatabase>
{
    public const string Name = "PostgreSQL";
}
