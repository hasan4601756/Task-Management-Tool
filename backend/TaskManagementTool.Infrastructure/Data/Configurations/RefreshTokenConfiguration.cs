using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Infrastructure.Identity;

namespace TaskManagementTool.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.Property(rt => rt.RevokedAt);

            builder.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}