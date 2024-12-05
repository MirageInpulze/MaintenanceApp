namespace MaintenanceApp.ViewModels;

public class ChangePasswordVM
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}