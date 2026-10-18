using SwimSchool.Application.Students.Dtos;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.Students;

public class StudentRequestValidationTests
{
    private const string BirthDate = nameof(CreateStudentRequest.BirthDate);

    // 18.10.2026 at 22:30 UTC = 19.10.2026 at 01:30 in Israel
    private static readonly FakeClock AfterMidnightInIsrael = new(new DateTime(2026, 10, 18, 22, 30, 0, DateTimeKind.Utc));

    private static CreateStudentRequest Request(DateOnly birthDate) => new()
    {
        IdentityNumber = "123456782",
        FullName = "נועה כהן",
        Phone = "0501234567",
        BirthDate = birthDate,
        HealthFund = HealthFund.Maccabi
    };

    [Fact]
    public void BirthDateInThePast_IsValid()
    {
        Assert.Empty(Validation.InvalidMembers(Request(new DateOnly(2015, 3, 1))));
    }

    // "Today" is the Israel date: 19.10 is already today in Israel, so it is not "in the past"
    [Fact]
    public void BirthDateOfTodayInIsrael_IsRejected_AndYesterdayIsAccepted()
    {
        Assert.Contains(BirthDate, Validation.InvalidMembers(Request(new DateOnly(2026, 10, 19)), AfterMidnightInIsrael));
        Assert.Empty(Validation.InvalidMembers(Request(new DateOnly(2026, 10, 18)), AfterMidnightInIsrael));
    }
}
