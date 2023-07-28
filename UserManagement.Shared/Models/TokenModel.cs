namespace UserManagement.Shared.Models;

public class TokenModel
{
    public Guid UserId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
