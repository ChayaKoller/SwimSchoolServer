using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Enrollments;
using SwimSchool.Infrastructure.Persistence;
using SwimSchool.Infrastructure.Persistence.Repositories;
using SwimSchool.Infrastructure.Time;

namespace SwimSchool.Infrastructure.Tests.TestSupport;

// The real Application service on top of the real repositories, for one user with her own DbContext
public static class AppServices
{
    public static EnrollmentService Enrollments(SwimSchoolDbContext db, ICurrentUser me) => new(
        new StudentRepository(db),
        new GroupRepository(db),
        new EnrollmentRepository(db),
        new UnitOfWork(db),
        me,
        new SystemDateTimeProvider());
}
