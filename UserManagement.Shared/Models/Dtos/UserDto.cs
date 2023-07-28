using System.ComponentModel.DataAnnotations;

namespace UserManagement.Shared.Models.Dtos;

public class UserDto
{
    [Required]
    public Guid Id { get; set; }
    [Required(ErrorMessage="User is required")]
    [MaxLength(50,ErrorMessage="UserName must be at least 1-50")]
    public string? UserName { get; set; }
    [Required(ErrorMessage="Role is required")]
    public string? Roles { get; set; }
}
