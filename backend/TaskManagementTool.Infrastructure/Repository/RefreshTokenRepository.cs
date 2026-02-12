using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Infrastructure.Data;
using TaskManagementTool.Infrastructure.Identity;

namespace TaskManagementTool.Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenRepository(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<bool> AddAsync(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return false;
            RefreshToken refreshToken = new RefreshToken
            {
                TokenHash = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task RevokeAllForUserAsync(string userId)
        {
            var tokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.RevokedAt = DateTime.UtcNow;

            _dbContext.RefreshTokens.Update(token);

            await _dbContext.SaveChangesAsync();
        }
    }
}