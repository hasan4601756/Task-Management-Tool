using Microsoft.AspNetCore.Identity;

namespace TaskManagementTool.Infrasrtucure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
    }
}