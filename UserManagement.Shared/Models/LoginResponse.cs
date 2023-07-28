namespace UserManagement.Shared.Models;

public class LoginResponse
{
   public Guid UserId { get; set; }
   public string? UserName { get; set; }
   public string? Roles { get; set; } 
   public string? AccessToken { get; set; }
   public string? RefreshToken { get; set; } 
}
