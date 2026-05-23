using Microsoft.AspNetCore.Identity;
using ShainingOpt.Models;
using ShainingOpt.ViewModels.Account;
using System.Security.Claims;

namespace ShainingOpt.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ProfileViewModel> BuildProfileViewModelAsync(User user, UpdateProfileDataViewModel model = null);
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal);
        Task<string> GetTokenAsync(User user);
        Task<bool> IsInRoleAsync(User? user, string v);
        Task<IdentityResult> LoginUserAsync(LoginViewModel model);
        Task Logout();
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task<IdentityResult> ResetPasswordAsync(User user, NewPasswordViewModel model);
        Task<IdentityResult> UpdateSecurityData(User user, UpdateSecurityDataViewModel model);
        Task<IdentityResult> UpdateUserAndCompanyDataAsync(User user, UpdateProfileDataViewModel model);
        Task<User?> UserExsiting(string email);
    }
}