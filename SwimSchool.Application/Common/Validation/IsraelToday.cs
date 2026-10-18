using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Interfaces;

namespace SwimSchool.Application.Common.Validation;

// "Today" for date validation is the Israel date, not the UTC date
internal static class IsraelToday
{
    private static readonly TimeZoneInfo IsraelZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem");

    // MVC passes the request services, so the API uses the registered clock (and tests a fake one).
    // Without a clock (e.g. validation outside MVC), the real Israel date is used.
    public static DateOnly From(ValidationContext context) =>
        context.GetService(typeof(IDateTimeProvider)) is IDateTimeProvider clock
            ? clock.TodayIsrael
            : DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IsraelZone));
}
