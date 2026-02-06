using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Domain.Enums;
using TaskManagementTool.Infrastructure.Identity;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.TaskItemId);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.DueDate)
            .IsRequired();

        builder.Property(t => t.CreationDate)
            .IsRequired();

        builder.Property(t => t.TaskStatus)
            .IsRequired();

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasDefaultValue(TaskPriority.Low);

        builder.Property(t => t.isActive)
            .HasDefaultValue(true);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}