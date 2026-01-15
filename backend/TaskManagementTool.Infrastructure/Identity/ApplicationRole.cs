using Microsoft.AspNetCore.Identity;

namespace TaskManagementTool.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
    }
}