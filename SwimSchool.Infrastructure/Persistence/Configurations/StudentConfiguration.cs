using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.IdentityNumber).HasMaxLength(9).IsRequired();
        builder.HasIndex(s => s.IdentityNumber).IsUnique();

        builder.Property(s => s.FullName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(20).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(254);
        builder.Property(s => s.PlacementRequest).HasMaxLength(500);
        builder.Property(s => s.HealthFund).HasConversion<int>();

        builder.HasOne(s => s.ResponsibleTeacher)
            .WithMany(u => u.Students)
            .HasForeignKey(s => s.ResponsibleTeacherId);
    }
}
