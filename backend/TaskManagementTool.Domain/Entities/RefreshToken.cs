namespace TaskManagementTool.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string TokenHash { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
    }
}