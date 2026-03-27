namespace SetlistManager.Common.Models;

public class TokenCreateModel
{
    public ProviderEnum Provider { get; set; }
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; }
}