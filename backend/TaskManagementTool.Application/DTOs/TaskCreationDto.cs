namespace TaskManagementTool.Application.DTOs{
    public class TaskCreationDto{
        public string Title {get; set;}
        public string? Description {get; set;}
        public DateOnly DueDate {get; set;}
        public int CategoryId {get; set;}
    }
}