namespace TaskManagementTool.Application.DTOs{
    public class TaskUpdationDto{
        public int Id {get; set;}
        public string Title {get; set;}
        public string? Description {get; set;}
        public DateOnly DueDate {get; set;}
        public TaskStatus Status {get; set;}
        public int CategoryId {get; set;}
    }
}