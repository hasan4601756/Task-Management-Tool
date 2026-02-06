using Microsoft.AspNetCore.Identity;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<TaskItem> Tasks { get; set; }
        public bool isActive {get; set;} = true;
    }
}