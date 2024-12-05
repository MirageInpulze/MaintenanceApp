using System.ComponentModel.DataAnnotations;

namespace MaintenanceApp.ViewModels;

public class LoginVm
{
    public string? Email { get; set; }
    
    public string? UserName { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public bool IsEmailOrUserNameProvided()
    {
        return !string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(UserName);
    }
}