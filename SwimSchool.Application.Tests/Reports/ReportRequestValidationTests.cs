using SwimSchool.Application.Reports;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.Reports;

// The date rules live on the DTO (Data Annotations), so they are checked the way the API checks them
public class ReportRequestValidationTests
{
    private const string Dates = nameof(CreateReportRequest.LessonDates);

    // FakeClock: 18.10.2026 at 12:00 in Israel
    private static readonly DateOnly Today = new(2026, 10, 18);

    // 18.10.2026 at 22:30 UTC = 19.10.2026 at 01:30 in Israel
    private static readonly FakeClock AfterMidnightInIsrael = new(new DateTime(2026, 10, 18, 22, 30, 0, DateTimeKind.Utc));

    private static CreateReportRequest Request(List<DateOnly> dates) => new()
    {
        StudentId = 20,
        TreatmentType = TreatmentType.Group,
        PricePerTreatment = 90m,
        ServiceDescription = ReportDefaults.ServiceDescription,
        LessonDates = dates
    };

    private static List<DateOnly> PastDates(int count) =>
        Enumerable.Range(0, count).Select(i => Today.AddDays(-i)).ToList();

    [Fact]
    public void ValidRequest_WithUpTo34Dates_UpToToday_HasNoErrors()
    {
        Assert.Empty(Validation.InvalidMembers(Request(PastDates(34))));
    }

    [Fact]
    public void MoreThan34Dates_IsRejected()
    {
        Assert.Contains(Dates, Validation.InvalidMembers(Request(PastDates(35))));
    }

    [Fact]
    public void DuplicateDates_AreRejected()
    {
        Assert.Contains(Dates, Validation.InvalidMembers(Request([Today, Today])));
    }

    [Fact]
    public void FutureDates_AreRejected()
    {
        Assert.Contains(Dates, Validation.InvalidMembers(Request([Today, Today.AddDays(1)])));
    }

    [Fact]
    public void NoDates_IsRejected()
    {
        Assert.Contains(Dates, Validation.InvalidMembers(Request([])));
    }

    // The bug that was fixed: at 01:30 in Israel, UTC is still "yesterday"
    [Fact]
    public void JustAfterMidnightInIsrael_TodaysIsraelDateIsAccepted_AndTomorrowIsNot()
    {
        Assert.Empty(Validation.InvalidMembers(Request([new DateOnly(2026, 10, 19)]), AfterMidnightInIsrael));
        Assert.Contains(Dates, Validation.InvalidMembers(Request([new DateOnly(2026, 10, 20)]), AfterMidnightInIsrael));
    }
}
