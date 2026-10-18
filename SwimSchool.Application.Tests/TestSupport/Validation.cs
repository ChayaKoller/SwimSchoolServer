using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using SwimSchool.Application.Common.Interfaces;

namespace SwimSchool.Application.Tests.TestSupport;

// Runs the Data Annotations the way the API does: the clock comes from DI
public static class Validation
{
    public static List<string> InvalidMembers(object request, IDateTimeProvider? clock = null)
    {
        using var services = new ServiceCollection()
            .AddSingleton(clock ?? new FakeClock())
            .BuildServiceProvider();

        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request, services, null), results, validateAllProperties: true);
        return results.SelectMany(r => r.MemberNames).ToList();
    }
}
