using MaintenanceApp.Models;
using MaintenanceApp.Services.Implement;
using Microsoft.AspNetCore.Identity;
using MaintenanceApp.Models;
using MaintenanceApp.ViewModels;

namespace MaintenanceApp.Services.Implement;

public interface IUserService : IServiceBase<User>
{
    Task<UserVm> AddUserAsync(UserVm userVm);
    Task<UserVm> UpdateUserAsync(UserVm UserVM);
    Task<UserVm> DeleteAsync(Guid id);

    Task<UserVm> GetUserAsync(Guid id);
    Task<IdentityResult> ResetUserPasswordAsync(Guid userId);

    Task<IdentityResult> ChangePassword(ChangePasswordVM model);
    
    Task<Boolean> CheckEmailExist(string email);
    Task<Boolean> CheckUsernameExist(string username);

}