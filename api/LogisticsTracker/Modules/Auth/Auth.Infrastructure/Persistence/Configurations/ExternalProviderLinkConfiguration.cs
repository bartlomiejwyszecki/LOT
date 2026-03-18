using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configurations;

public class ExternalProviderLinkConfiguration : IEntityTypeConfiguration<ExternalProviderLink>
{
    public void Configure(EntityTypeBuilder<ExternalProviderLink> builder)
    {
        builder.ToTable("ExternalProviderLinks");

        builder.HasKey(link => link.Id);

        builder.Property(link => link.UserId)
            .IsRequired();

        builder.Property(link => link.Provider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(link => link.ExternalUserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(link => link.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(link => link.LinkedAt)
            .IsRequired();

        builder.Property(link => link.CreatedAt)
            .IsRequired();

        builder.Property(link => link.UpdatedAt);

        builder.HasIndex(link => new { link.Provider, link.ExternalUserId })
            .IsUnique();

        builder.HasIndex(link => link.UserId);
    }
}