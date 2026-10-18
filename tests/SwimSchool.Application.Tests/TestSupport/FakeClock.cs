using SwimSchool.Application.Common.Interfaces;

namespace SwimSchool.Application.Tests.TestSupport;

// A fixed time, so tests never depend on the day they run.
// Israel is treated as a fixed UTC+3, which is enough for the tests.
public sealed class FakeClock(DateTime? utcNow = null) : IDateTimeProvider
{
    public static readonly DateTime Now = new(2026, 10, 18, 9, 0, 0, DateTimeKind.Utc);

    public DateTime UtcNow { get; } = utcNow ?? Now;
    public DateOnly TodayIsrael => DateOnly.FromDateTime(UtcToIsrael(UtcNow));
    public DateTime IsraelToUtc(DateTime israelLocal) => DateTime.SpecifyKind(israelLocal.AddHours(-3), DateTimeKind.Utc);
    public DateTime UtcToIsrael(DateTime utc) => DateTime.SpecifyKind(utc.AddHours(3), DateTimeKind.Unspecified);
}
