using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class VerifyEmailModel
{
    [Required]
    public string Email { get; set; } = default!;
    [Required]
    public string Token { get; set; } = default!;
}