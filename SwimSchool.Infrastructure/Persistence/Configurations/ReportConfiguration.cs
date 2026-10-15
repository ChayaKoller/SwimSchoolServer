using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(r => r.HealthFund).HasConversion<int>();
        builder.Property(r => r.LessonDates).HasColumnType("date[]");

        builder.HasOne(r => r.Student)
            .WithMany(s => s.Reports)
            .HasForeignKey(r => r.StudentId);

        builder.HasOne(r => r.CreatedBy)
            .WithMany()
            .HasForeignKey(r => r.CreatedById);
    }
}
