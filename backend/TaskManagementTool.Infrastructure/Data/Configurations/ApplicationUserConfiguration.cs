using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Infrastructure.Identity;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id); // IdentityUser already has Id

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(200);
    }
}