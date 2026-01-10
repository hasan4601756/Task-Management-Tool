namespace TaskManagementTool.Domain.Entities
{
    public class TaskCategory
    {
        public int TaskCategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}