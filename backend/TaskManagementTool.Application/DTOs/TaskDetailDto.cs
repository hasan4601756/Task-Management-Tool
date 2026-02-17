using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Application.DTOs{
    public class TaskDetailDto{
        public int Id {get; set;}
        public string Title {get; set;}
        public string? Description {get; set;}
        public DateOnly DueDate {get; set;}
        public DateTime CreationDate {get; set;}
        public TaskItemStatus TaskStatus {get; set;}
        public int CategoryId {get; set;}
        public string CategoryName {get; set;}
        public string? CategoryDescription {get; set;}
        public TaskPriority Priority {get; set;}
    }
}