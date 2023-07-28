using System.ComponentModel.DataAnnotations;

namespace UserManagement.Shared.Models;

public class UserLogin
{
    [Required, MaxLength(50)]
    public string? UserName { get; set; }
    [Required, MaxLength(255)]
    public string? Password { get; set; }
}
