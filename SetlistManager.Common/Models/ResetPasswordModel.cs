using System.ComponentModel.DataAnnotations;
namespace SetlistManager.Common.Models;

public  class ResetPasswordModel
{
    [Required]
    public string Email { get; set; } = default!;
    [Required]
    public string NewPassword { get; set; } = default!;
    [Required]
    public string Token { get; set; } = default!;
}