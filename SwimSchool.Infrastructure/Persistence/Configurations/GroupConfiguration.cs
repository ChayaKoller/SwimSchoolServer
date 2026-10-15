using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Group_Capacity",
            "\"MaxStudents\" > 0 AND \"EnrolledCount\" >= 0 AND \"EnrolledCount\" <= \"MaxStudents\""));

        builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Price).HasPrecision(10, 2);
        builder.Property(g => g.Season).HasConversion<int>();

        builder.Property(g => g.Version).IsRowVersion();

        builder.HasOne(g => g.Teacher)
            .WithMany(u => u.Groups)
            .HasForeignKey(g => g.TeacherId);
    }
}
