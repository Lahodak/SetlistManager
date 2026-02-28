using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class VerifyModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    [Required]
    public string Token { get; set; } = default!;
}