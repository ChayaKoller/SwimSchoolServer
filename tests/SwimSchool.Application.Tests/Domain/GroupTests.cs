using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Exceptions;

namespace SwimSchool.Application.Tests.Domain;

// The spot rules that live on the entity itself
public class GroupTests
{
    [Fact]
    public void ReserveSpot_WhenFull_Throws()
    {
        var group = TestData.Group(maxStudents: 3, enrolledCount: 3);

        Assert.Throws<DomainException>(group.ReserveSpot);
        Assert.Equal(3, group.EnrolledCount);
    }

    [Fact]
    public void ReserveSpot_WhenInactive_Throws()
    {
        Assert.Throws<DomainException>(TestData.Group(isActive: false).ReserveSpot);
    }

    [Fact]
    public void ReleaseSpot_NeverGoesBelowZero()
    {
        var group = TestData.Group(enrolledCount: 0);

        group.ReleaseSpot();

        Assert.Equal(0, group.EnrolledCount);
    }

    [Theory]
    [InlineData(10, 9, true)]
    [InlineData(10, 10, false)]
    public void HasFreeSpot_ComparesEnrolledToMax(int max, int enrolled, bool expected)
    {
        Assert.Equal(expected, TestData.Group(maxStudents: max, enrolledCount: enrolled).HasFreeSpot());
    }
}
