using System.ComponentModel.DataAnnotations;

namespace UserManagement.Shared.Models;

public class User
{
    [Required, Key]
    public Guid Id { get; set; }
    [Required, StringLength(50,MinimumLength=3,ErrorMessage="UserName must be between 3 and 50 characters!")]
    public string? UserName { get; set; }
    [Required]
    public string? Password { get; set; }
    public virtual ICollection<Role>? Roles { get; set; }
}
