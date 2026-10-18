using Moq;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Reports;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Application.Reports.Pdf;
using SwimSchool.Application.Students;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.Reports;

public class ReportServiceTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IReportRepository> _reports = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ReportService CreateService(ICurrentUser me, params IReportPdfGenerator[] generators) => new(
        _students.Object, _reports.Object, _unitOfWork.Object, me, new FakeClock(), TestMapper.Instance, generators);

    private static CreateReportRequest Request(int studentId, decimal price) => new()
    {
        StudentId = studentId,
        TreatmentType = TreatmentType.Group,
        PricePerTreatment = price,
        ServiceDescription = "  " + ReportDefaults.ServiceDescription + "  ",
        LessonDates = [new DateOnly(2026, 10, 12), new DateOnly(2026, 10, 5)]
    };

    [Fact]
    public async Task Create_SavesTheEnteredPriceAndTheCreator_EvenWhenDifferentFromTheDefault()
    {
        var student = TestData.Student();
        _students.Setup(r => r.GetByIdAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        Report? added = null;
        _reports.Setup(r => r.Add(It.IsAny<Report>())).Callback<Report>(r => added = r);
        _reports.Setup(r => r.GetByIdAsync(It.IsAny<int>(), TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(() => added);
        var secretary = FakeCurrentUser.SecretaryOf(TestData.TeacherId);

        await CreateService(secretary).CreateAsync(Request(student.Id, price: 75m));

        Assert.NotNull(added);
        Assert.Equal(75m, added.PricePerTreatment);
        Assert.Equal(secretary.UserId, added.CreatedById);
        Assert.Equal(student.HealthFund, added.HealthFund);
        Assert.Equal(ReportDefaults.ServiceDescription, added.ServiceDescription);
        Assert.Equal([new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 12)], added.LessonDates);
        Assert.Null(added.InsuranceCompany);
    }

    [Fact]
    public async Task Create_ForAnotherTeachersStudent_ThrowsStudentNotFound()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId)).CreateAsync(Request(studentId: 99, price: 90m)));

        Assert.Equal(ErrorCodes.StudentNotFound, ex.Code);
        _reports.Verify(r => r.Add(It.IsAny<Report>()), Times.Never);
    }

    [Theory]
    [InlineData(null, HealthFund.Maccabi, "maccabi")]   // the fund has its own form
    [InlineData(null, HealthFund.Clalit, "general")]    // no form for this fund yet
    [InlineData("הפניקס", HealthFund.Maccabi, "general")] // private insurance always uses the general form
    public async Task GetPdf_PicksTheFundsFormOrTheGeneralOne(string? insurance, HealthFund fund, string expected)
    {
        var teacher = TestData.Teacher();
        var report = new Report
        {
            Id = 7,
            Student = TestData.Student(),
            CreatedBy = teacher,
            HealthFund = fund,
            InsuranceCompany = insurance,
            PricePerTreatment = 90m,
            IssuedAt = FakeClock.Now,
            LessonDates = [new DateOnly(2026, 10, 5)]
        };
        _reports.Setup(r => r.GetForPdfAsync(report.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(report);
        var service = CreateService(FakeCurrentUser.Teacher(TestData.TeacherId),
            new FakePdfGenerator(HealthFund.Maccabi, "maccabi"), new FakePdfGenerator(null, "general"));

        var file = await service.GetPdfAsync(report.Id);

        Assert.Equal(expected, System.Text.Encoding.UTF8.GetString(file.Content));
        Assert.Equal("report-7.pdf", file.FileName);
    }

    private sealed class FakePdfGenerator(HealthFund? fund, string name) : IReportPdfGenerator
    {
        public HealthFund? Fund => fund;
        public byte[] Generate(ReportPdfData data) => System.Text.Encoding.UTF8.GetBytes(name);
    }
}
