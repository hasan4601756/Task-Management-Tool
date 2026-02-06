using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Domain.Entities
{
    public class TaskItem
    {
        public int TaskItemId {get; set;}
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public DateOnly DueDate {get; set;}
        public DateTime CreationDate {get; set;}
        public TaskItemStatus TaskStatus {get; set;}
        public bool isActive {get; set;} = true;
        public TaskPriority Priority { get; set; }
        public string AssignedUserId { get; set; }
        public int TaskCategoryId {get; set;}
        public TaskCategory Category {get; set;}
    }
}