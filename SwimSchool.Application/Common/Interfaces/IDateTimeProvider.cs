namespace SwimSchool.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly TodayIsrael { get; }
    DateTime IsraelToUtc(DateTime israelLocal);
    DateTime UtcToIsrael(DateTime utc);
}
