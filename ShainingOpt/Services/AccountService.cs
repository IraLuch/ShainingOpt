using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Mappers;
using ShainingOpt.Models;
using ShainingOpt.ViewModels;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace ShainingOpt.Services
{
    public class AccountService
    {


        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        public AccountService(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed();

            }
            var clientRole = await _roleManager.FindByNameAsync("Client");
            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                RoleId = clientRole.Id
                
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;

            }

                await _userManager.AddToRoleAsync(user, "Client");

                var company = new Company
                {
                    UserId = user.Id,
                    CompanyName = model.CompanyName,
                    Inn = model.Inn,
                    Kpp = model.Kpp,
                    LegalAddress = model.LegalAddress,
                    ContactPerson = model.ContactPerson
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                return IdentityResult.Success;

                
            

        }

        public async Task<IdentityResult> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return IdentityResult.Failed();
            }

            var res = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!res.Succeeded)
            {
                return IdentityResult.Failed();
            }
            return IdentityResult.Success;
    }

        internal async Task<User?> UserExsiting(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        internal async Task<string> GetTokenAsync(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        internal async Task<IdentityResult> ResetPasswordAsync(User user, NewPasswordViewModel model)
        {
            var res = await _userManager.ResetPasswordAsync(user, model.Token ,model.NewPassword);
            return res;

        }

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            if (user == null)
            {
                return null;
            }
            return await _context.Users.Include(c => c.Company).FirstOrDefaultAsync(u => user.Id == u.Id);
        }

        internal async Task<IdentityResult> UpdateUserAndCompanyDataAsync(User user, UpdateProfileDataViewModel model)
        {

            try
            {

                ProfileMapper.UpdateUser(user, model);
                ProfileMapper.UpdateCompany(user, model);

                var result = await _userManager.UpdateAsync(user);

                return result;
            }
            catch 
            {
                return IdentityResult.Failed();
            }
        }

        internal async Task<IdentityResult> UpdateSecurityData(User user, UpdateSecurityDataViewModel model)
        {
            var res = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword) ;
            return res;
        }

        internal async Task<ProfileViewModel> BuildProfileViewModelAsync(User user, UpdateProfileDataViewModel model = null)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(u => u.User.Id == user.Id);
            if (model != null)
            {
                return new ProfileViewModel
                {
                    Id = company.CompanyId,
                    CompanyName = model.CompanyName,
                    ContactPerson = model.ContactPerson,
                    Email = model.Email,
                    Phone = model.Phone,
                    Inn = model.Inn,
                    Kpp = model.Kpp,
                    Address = model.Address
                };
            }
            return new ProfileViewModel
            {
                Id = company.CompanyId,
                CompanyName = company.CompanyName,
                ContactPerson = company.ContactPerson,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Inn = company.Inn,
                Kpp = company.Kpp,
                Address = company.LegalAddress,


                Password = "",
                NewPassword = "",
                ConfirmPassword = ""
            };
        }

        internal async Task Logout()
        {
           await _signInManager.SignOutAsync();
        }

        internal async Task<bool> IsInRoleAsync(User? user, string v)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return await _userManager.IsInRoleAsync(user, v);
        }
    } }

