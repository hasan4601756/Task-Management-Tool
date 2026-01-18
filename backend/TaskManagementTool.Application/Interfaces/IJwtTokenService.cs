namespace TaskManagementTool.Application
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(string email);
    }
}