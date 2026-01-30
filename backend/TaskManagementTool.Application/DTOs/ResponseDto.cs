namespace TaskManagementTool.Application.DTOs{
    public class ResponseDto
    {
        public bool Succeeded {get; set;}
        public IEnumerable<string>? Errors { get; set; }
    }
}