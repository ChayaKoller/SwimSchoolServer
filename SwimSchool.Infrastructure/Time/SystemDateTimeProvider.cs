using SwimSchool.Application.Common.Interfaces;

namespace SwimSchool.Infrastructure.Time;

public class SystemDateTimeProvider : IDateTimeProvider
{
    private static readonly TimeZoneInfo IsraelZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem");

    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly TodayIsrael => DateOnly.FromDateTime(UtcToIsrael(UtcNow));

    // Each value is converted on its own, so the DST offset of that specific date is used
    public DateTime IsraelToUtc(DateTime israelLocal) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(israelLocal, DateTimeKind.Unspecified), IsraelZone);

    public DateTime UtcToIsrael(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), IsraelZone);
}
