using Microsoft.EntityFrameworkCore;
using TaskManagementTool.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TaskManagementTool.Infrastructure.Identity;

namespace TaskManagementTool.Infrastructure.Data
{ 
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>(); 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
    }
}