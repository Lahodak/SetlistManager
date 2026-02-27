namespace SetlistManager.Common.Models;

public class TokenModel
{
    public int Id { get; set; }
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public string Provider { get; set; } = default!;
}