using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Application.DTOs{
    public class TaskDto
    {
        public int Id {get; set;}
        required public string Title {get; set;}
        public TaskItemStatus TaskStatus {get; set;}
        public TaskPriority Priority {get; set;}
        public string? UserName { get;set;}
    }
}