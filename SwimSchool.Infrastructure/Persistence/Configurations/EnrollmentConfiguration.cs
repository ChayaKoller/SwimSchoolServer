using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.Property(e => e.AmountDue).HasPrecision(10, 2);
        builder.Property(e => e.AmountPaid).HasPrecision(10, 2);

        builder.HasIndex(e => new { e.StudentId, e.GroupId }).IsUnique();

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        builder.HasOne(e => e.Group)
            .WithMany(g => g.Enrollments)
            .HasForeignKey(e => e.GroupId);
    }
}
