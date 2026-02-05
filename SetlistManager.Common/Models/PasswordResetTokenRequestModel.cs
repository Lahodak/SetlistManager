using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class PasswordResetRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}