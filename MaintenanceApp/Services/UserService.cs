using System.Text;
using Microsoft.AspNetCore.Identity;
using MaintenanceApp.Exceptions;
using MaintenanceApp.Infrastructure;
using MaintenanceApp.Models;
using MaintenanceApp.ViewModels;

namespace MaintenanceApp.Services.Implement;

public class UserService(
    IUnitOfWork _unitOfWork,
    ILogger<UserService> _logger,
    UserManager<User> _userManager,
    RoleManager<Role> _roleManager

)
    : ServiceBase<User>(_unitOfWork, _logger), IUserService
{

    public async Task<UserVm> AddUserAsync(UserVm userVm)
    {
        if ((await _userManager.FindByEmailAsync(userVm.Email)) != null)
            throw new BadRequestException("Email already exists");

        if ((await _userManager.FindByNameAsync(userVm.UserName)) != null)
            throw new BadRequestException("UserName already exists");

        User user = new User()
        {
            UserName = userVm.UserName,
            Email = userVm.Email,
            IsActive = true,
        };

        var result = await _userManager.CreateAsync(user, userVm.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, userVm.Role ?? "customer");

            User tmpUser = await _userManager.FindByNameAsync(userVm.UserName);
            var tmpRoles = await _userManager.GetRolesAsync(tmpUser);

            return new UserVm
            {
                Id = tmpUser.Id,
                UserName = tmpUser.UserName,
                Email = tmpUser.Email,
                FullName = tmpUser.FullName,
                Role = tmpRoles.FirstOrDefault() ?? "No Role Assigned",
            };
        }
        else
        {
            throw new Exception("Tạo người dùng staff thất bại: " +
                                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<UserVm> UpdateUserAsync(UserVm userVm)
    {
        var existingUser = await _userManager.FindByIdAsync(userVm.Id.ToString());
        if (existingUser == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        // Kiểm tra email và UserName
        var existingEmailUser = await _userManager.FindByEmailAsync(userVm.Email);
        if (existingEmailUser != null && existingEmailUser.Id != userVm.Id)
        {
            throw new BadRequestException("Email already exists");
        }

        var existingUserNameUser = await _userManager.FindByNameAsync(userVm.UserName);
        if (existingUserNameUser != null && existingUserNameUser.Id != userVm.Id)
        {
            throw new BadRequestException("UserName already exists");
        }

        // Cập nhật thông tin và thêm SecurityStamp nếu thiếu
        existingUser.UserName = userVm.UserName;
        existingUser.Email = userVm.Email;
        existingUser.FullName = userVm.FullName;
        existingUser.SecurityStamp ??= Guid.NewGuid().ToString();

        // Cập nhật vai trò nếu Role khác null
        if (!string.IsNullOrEmpty(userVm.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(existingUser);

            // Loại bỏ các vai trò hiện tại của người dùng
            if (currentRoles.Count > 0)
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    throw new Exception("Failed to remove existing roles");
                }
            }

            // Thêm vai trò mới
            var addRoleResult = await _userManager.AddToRoleAsync(existingUser, userVm.Role);
            if (!addRoleResult.Succeeded)
            {
                throw new Exception("Failed to add new role");
            }
        }

        IdentityResult result = await _userManager.UpdateAsync(existingUser);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to update user");
        }

        // Trả về đối tượng UserVm sau khi cập nhật
        return new UserVm
        {
            Id = existingUser.Id,
            UserName = existingUser.UserName,
            Email = existingUser.Email,
            FullName = existingUser.FullName,
            Role = userVm.Role // Cập nhật lại vai trò trong UserVm
        };
    }

    Task<UserVm> IUserService.DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<User> DeleteAsync(Guid id)
    {
        User tmpUser = await _userManager.FindByIdAsync(id.ToString());

        if (tmpUser == null)
        {
            return null;
        }

        tmpUser.UserName = tmpUser.UserName + $"_{tmpUser.Id}";
        tmpUser.Email = tmpUser.Email + $"_{tmpUser.Id}";
        tmpUser.IsActive = false;

        IdentityResult result = await _userManager.UpdateAsync(tmpUser);

        return result.Succeeded ? tmpUser : null;
    }

    public async Task<UserVm> GetUserAsync(Guid id)
    {
        // Tìm kiếm người dùng với ID được cung cấp
        User tmpUser = await _userManager.FindByIdAsync(id.ToString());

        if (tmpUser == null)
        {
            return null; // Trả về null nếu người dùng không tồn tại
        }

        // Chuyển đổi User sang UserVm
        var userVm = new UserVm
        {
            Id = tmpUser.Id,
            UserName = tmpUser.UserName,
            Email = tmpUser.Email,
            FullName = tmpUser.FullName, 
        };

        // Lấy vai trò của người dùng
        var roles = await _userManager.GetRolesAsync(tmpUser);
        userVm.Role = roles.FirstOrDefault(); // Gán vai trò đầu tiên nếu có

        return userVm;
    }
    
    public async Task<IdentityResult> ResetUserPasswordAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        // Tạo mật khẩu tạm thời
        var temporaryPassword = "Test@123";

        // Xóa mật khẩu cũ
        var removePasswordResult = await _userManager.RemovePasswordAsync(user);

        if (!removePasswordResult.Succeeded)
        {
            return removePasswordResult;
        }

        // Thêm mật khẩu tạm thời
        var addPasswordResult = await _userManager.AddPasswordAsync(user, temporaryPassword);


        return addPasswordResult.Succeeded ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> ChangePassword(ChangePasswordVM model)
    {
        // var user = await _userManager.GetUserAsync(User);
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            return IdentityResult.Failed(new IdentityError { Description = result.Errors.ToString() });
        }

        return result.Succeeded ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<bool> CheckEmailExist(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    } 
    
    public async Task<bool> CheckUsernameExist(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user is not null;
    }
    
    
    
    private static string GenerateRandomPassword(IdentityOptions options)
    {
        var random = new Random();
        var passwordBuilder = new StringBuilder();

        // Đảm bảo có ít nhất 1 chữ cái viết hoa
        passwordBuilder.Append((char)random.Next('A', 'Z' + 1));

        // Đảm bảo có ít nhất 1 chữ cái viết thường
        passwordBuilder.Append((char)random.Next('a', 'z' + 1));

        // Đảm bảo có ít nhất 1 số
        passwordBuilder.Append(random.Next(0, 10));

        // Đảm bảo có ít nhất 1 ký tự đặc biệt
        var specialChars = "!@#$%^&*()_+[]{}|;:,.<>?";
        passwordBuilder.Append(specialChars[random.Next(specialChars.Length)]);

        // Thêm các ký tự ngẫu nhiên khác cho đến khi đạt độ dài tối thiểu
        while (passwordBuilder.Length < options.Password.RequiredLength)
        {
            // Chọn ngẫu nhiên từ 4 loại ký tự
            char nextChar;
            int charType = random.Next(4);
            switch (charType)
            {
                case 0: // Ký tự viết hoa
                    nextChar = (char)random.Next('A', 'Z' + 1);
                    break;
                case 1: // Ký tự viết thường
                    nextChar = (char)random.Next('a', 'z' + 1);
                    break;
                case 2: // Ký tự số
                    nextChar = (char)random.Next('0', '9' + 1);
                    break;
                case 3: // Ký tự đặc biệt
                    nextChar = specialChars[random.Next(specialChars.Length)];
                    break;
                default:
                    nextChar = ' '; // Không bao giờ đạt đến đây
                    break;
            }
            passwordBuilder.Append(nextChar);
        }

        // Trộn mật khẩu để ngẫu nhiên hơn
        var password = passwordBuilder.ToString();
        return Shuffle(password);
    }

    private static string Shuffle(string password)
    {
        var random = new Random();
        var chars = password.ToCharArray();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            // Hoán đổi
            char temp = chars[i];
            chars[i] = chars[j];
            chars[j] = temp;
        }
        return new string(chars);
    }
    
}