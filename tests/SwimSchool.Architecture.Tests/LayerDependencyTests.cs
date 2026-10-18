using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace SwimSchool.Architecture.Tests;

// Fails the build's tests if a layer starts using a layer it must not know (Clean Architecture: dependencies point inward)
public class LayerDependencyTests
{
    private const string Domain = "SwimSchool.Domain";
    private const string Application = "SwimSchool.Application";
    private const string Infrastructure = "SwimSchool.Infrastructure";
    private const string Api = "SwimSchool.API";

    private static readonly Assembly DomainAssembly = typeof(SwimSchool.Domain.Common.BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(SwimSchool.Application.DependencyInjection).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(SwimSchool.Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly ApiAssembly = typeof(SwimSchool.API.Controllers.StudentsController).Assembly;

    [Fact]
    public void Domain_DependsOnNoOtherLayer_AndNoFramework()
    {
        AssertNoDependency(DomainAssembly,
            Application, Infrastructure, Api,
            "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Riok.Mapperly");
    }

    [Fact]
    public void Application_DoesNotDependOnInfrastructureOrApi()
    {
        AssertNoDependency(ApplicationAssembly,
            Infrastructure, Api,
            "Microsoft.EntityFrameworkCore", "Npgsql", "QuestPDF", "Microsoft.AspNetCore");
    }

    [Fact]
    public void Infrastructure_DoesNotDependOnApi()
    {
        AssertNoDependency(InfrastructureAssembly, Api, "Microsoft.AspNetCore.Mvc");
    }

    [Fact]
    public void Controllers_UseOnlyApplicationServices_NeverDbContextOrInfrastructure()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespace($"{Api}.Controllers")
            .ShouldNot().HaveDependencyOnAny(Infrastructure, "Microsoft.EntityFrameworkCore")
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void Controllers_InheritControllerBase_AndAreNamedController()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespace($"{Api}.Controllers")
            .Should().Inherit(typeof(ControllerBase))
            .And().HaveNameEndingWith("Controller")
            .GetResult();

        AssertSuccessful(result);
    }

    private static void AssertNoDependency(Assembly assembly, params string[] forbidden)
    {
        var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(forbidden).GetResult();

        AssertSuccessful(result);
    }

    // Lists the offending types in the failure message, so it is clear what to fix
    private static void AssertSuccessful(TestResult result) =>
        Assert.True(result.IsSuccessful,
            "Architecture rule broken by: " + string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>()));
}
