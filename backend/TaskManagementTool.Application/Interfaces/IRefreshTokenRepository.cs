using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> AddAsync(string token, string email);
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RevokeAsync(RefreshToken token);
        Task RevokeAllForUserAsync(string userId);
    }
}