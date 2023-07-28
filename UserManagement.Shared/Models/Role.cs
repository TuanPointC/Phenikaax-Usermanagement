using System.ComponentModel.DataAnnotations;

namespace UserManagement.Shared.Models;

public class Role
{
    [Required, Key] public Guid Id { get; set; } = new Guid();
    
    [Required,MaxLength(50)]
    public string? Name { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    public virtual ICollection<User>? Users { get; set; }
}
