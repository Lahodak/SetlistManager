namespace SetlistManager.Data.Entities;

public class Token : BaseEntity
{
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int ProviderId { get; set; }
    public virtual Provider Provider { get; set; } = default!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
}