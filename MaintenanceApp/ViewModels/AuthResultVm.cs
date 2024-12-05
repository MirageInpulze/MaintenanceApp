using MaintenanceApp.Models;

namespace MaintenanceApp.ViewModels;

public class AuthResultVm
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    
    public User? UserInformation { get; set; }
    
    public IList<String>? Roles { get; set; }
}