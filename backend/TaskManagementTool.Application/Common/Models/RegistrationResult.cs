namespace TaskManagementTool.Application.Common.Models
{
    public class RegistrationResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}