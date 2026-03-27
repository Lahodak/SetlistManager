using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class LoginResultModel
{
    [Required]
    public string Token { get; set; } = default!;
}