# SwimSchool server

.NET 8 Web API for a swimming school: registration, attendance, and health-fund reports. This system is for a real client and will be deployed to the cloud. PostgreSQL (Npgsql).

## Architecture: Clean Architecture (dependencies point inward only)
- **SwimSchool.Domain:** entities, enums, BaseEntity, and business rules that belong to a single entity (e.g. `Group.HasFreeSpot()`). NO project references, NO NuGet packages.
- **SwimSchool.Application:** references Domain only.
  - Repository interfaces and IUnitOfWork; Filters and PagedResult.
  - DTOs and Mapperly mappers (static, generated at build time); Services and their interfaces.
  - Business exceptions (NotFound, Conflict, Forbidden).
  - Interfaces for external concerns: ICurrentUser, IPasswordHasher, IJwtTokenGenerator, IReportPdfGenerator.
  - No EF Core, no ASP.NET, no IQueryable in public signatures.
  - DI entry point: `AddApplication()`.
- **SwimSchool.Infrastructure:** references Application.
  - DbContext, Configurations, Migrations, repository implementations and the DevelopmentDataSeeder.
  - Implementations of QuestPDF, JWT and password hashing.
  - Translates DbUpdateConcurrencyException to the Application ConflictException.
  - DI entry point: `AddInfrastructure(IConfiguration)`: reads ConnectionStrings:Default and the Jwt section.
- **SwimSchool.API (Presentation):** references Application and Infrastructure.
  - Thin controllers that call Application services only, returning `ActionResult<T>`.
  - Middleware, NLog and Swagger (Development only), and the ICurrentUser implementation (reads JWT claims).
  - Infrastructure is used ONLY in Program.cs via `AddInfrastructure(...)`. Never use the DbContext in controllers.
- Package versions live only in Directory.Packages.props. Never add a Version attribute to a csproj.

## Conventions
- One type per file, with file-scoped namespaces.
- `string.Empty` for strings, `null!` for required navigations, and `new List<T>()` for collections.
- All DateTime values are UTC.
- Every query is filtered by the "owning teacher": the teacher herself, or the secretary's TeacherId. `teacherId` is a required parameter on every repository read method. Another teacher's data returns 404.
- Use soft delete (IsActive), never hard delete. Every FK uses DeleteBehavior.Restrict.
- Concurrency on a group spot uses Group.Version (xmin). A conflict returns 409.
- Migrations: `dotnet ef migrations add <Name> -p SwimSchool.Infrastructure -s SwimSchool.API`

## Working rules
- Work in the steps given in the prompt. Before each step, list the planned files and commands briefly and WAIT for approval.
- Be concise. Do not explain code unless asked.
- Never put passwords or secrets in code, appsettings, or output. Do NOT touch user secrets.
- Do NOT commit. The developer commits herself.
- Do not add anything that the prompt does not ask for.
