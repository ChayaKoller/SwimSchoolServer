using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasIndex(a => new { a.LessonId, a.EnrollmentId }).IsUnique();

        builder.HasOne(a => a.Lesson)
            .WithMany(l => l.Attendances)
            .HasForeignKey(a => a.LessonId);

        builder.HasOne(a => a.Enrollment)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EnrollmentId);
    }
}
