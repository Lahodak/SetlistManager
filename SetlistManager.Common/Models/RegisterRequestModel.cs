using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class RegisterRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    [MinLength(9)]
    public string Password { get; set; } = null!;
}