using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.IdentityNumber).HasMaxLength(9).IsRequired();
        builder.HasIndex(u => u.IdentityNumber).IsUnique();

        builder.Property(u => u.FullName).HasMaxLength(100).IsRequired();

        builder.Property(u => u.Email).HasMaxLength(254).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Phone).HasMaxLength(20).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.SecurityStamp).HasMaxLength(32).IsRequired();
        builder.Property(u => u.Role).HasConversion<int>();

        builder.Ignore(u => u.OwningTeacherId);
        builder.Ignore(u => u.CanSignIn);

        builder.HasOne(u => u.Teacher)
            .WithMany(t => t.Secretaries)
            .HasForeignKey(u => u.TeacherId);
    }
}
