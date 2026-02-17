using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Application.DTOs{
    public class TaskUpdationDto{
        public int Id {get; set;}
        public string Title {get; set;}
        public string? Description {get; set;}
        public DateOnly DueDate {get; set;}
        public TaskItemStatus Status {get; set;}
        public int CategoryId {get; set;}
        public TaskPriority Priority {get; set;}
    }
}