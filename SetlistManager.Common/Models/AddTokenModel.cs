namespace SetlistManager.Common.Models;

public class AddTokenModel
{
    public ProviderEnum Provider { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; } = default!;
}