using MaintenanceApp.Models;
using MaintenanceApp.ViewModels;
using MaintenanceApp.ViewModels;

namespace MaintenanceApp.Services.Implement;

public interface IAuthenticationService
{
    Task<AuthResultVm> Login(LoginVm payload);
    Task<AuthResultVm> RefreshJwtToken(string refreshTokenId);
}