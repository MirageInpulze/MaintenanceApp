using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MaintenanceApp.Infrastructure;
using MaintenanceApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MaintenanceApp.Infrastructure;
using MaintenanceApp.Models;
using MaintenanceApp.ViewModels;
using MaintenanceApp.ViewModels;

namespace MaintenanceApp.Services.Implement;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
   
    public AuthenticationService(UserManager<User> userManager, RoleManager<Role> roleManager, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResultVm> Login(LoginVm payload)
    {
        User user = null;
        
        if (payload.Email != null)
        {
            user = await _userManager.FindByEmailAsync(payload.Email);
        }
        else if (payload.UserName != null)
        {
            user = await _userManager.FindByNameAsync(payload.UserName);
        }
        
        if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
        {
            var tokenValue = await GenerateJwtToken(user);
            return tokenValue;
        }
        return new AuthResultVm 
        {
            Token = null,
            RefreshToken = null
        };
    }
    
    private async Task<AuthResultVm> GenerateJwtToken(User user)
    {
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(10), // 5 - 10mins
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        
        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsRevoked = false,
            UserId = user.Id,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddDays(1),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        _unitOfWork.GetRepository<RefreshToken>().Add(refreshToken);
        await _unitOfWork.SaveChangesAsync();
        var response = new AuthResultVm()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo,
            RefreshTokenExpiresAt = refreshToken.DateExpire,
            UserInformation = user,
            Roles = userRoles,
        };
        return response;
    }

    public async Task<AuthResultVm> RefreshJwtToken(string refreshTokenId)
    {
        RefreshToken? refreshToken = _unitOfWork.GetRepository<RefreshToken>()
            .GetAll()
            .FirstOrDefault(x => x.Token.Equals(refreshTokenId));

        if (refreshToken == null)
        {
            throw new Exception($"refreshTokenId:{refreshTokenId} not found");
        }

        if (refreshToken.DateExpire < DateTime.UtcNow)
        {
            throw new Exception($"refreshTokenId:{refreshTokenId} expired");
        }
        
        User user = _unitOfWork.GetRepository<User>().GetById(refreshToken.UserId);
        
        return await GenerateJwtToken(user);
    }
}