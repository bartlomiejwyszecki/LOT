using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(user => user.Email, email =>
        {
            email.Property(value => value.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            email.HasIndex(value => value.Value)
                .IsUnique();
        });

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500);

        builder.Property(user => user.Role)
            .IsRequired();

        builder.Property(user => user.IsEmailVerified)
            .IsRequired();

        builder.Property(user => user.EmailVerificationToken)
            .HasMaxLength(50);

        builder.Property(user => user.PasswordResetToken)
            .HasMaxLength(50);

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt);
    }
}