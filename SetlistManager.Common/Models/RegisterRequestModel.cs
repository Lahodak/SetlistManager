using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class RegisterRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    [MinLength(9)]
    public string Password { get; set; } = default!;
}