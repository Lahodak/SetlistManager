namespace SetlistManager.Common.Models;

public class RegisterResultModel
{
    public bool Success { get; set; }    
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}