using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.ViewModels;
using System.Security.Claims;

namespace ShainingOpt.Services
{
    public class AccountService
    {


        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountService(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed();

            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return result;

            try
            {
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
                await transaction.CommitAsync();
                return IdentityResult.Success;

            }
            catch
            {
                await transaction.RollbackAsync();
                return IdentityResult.Failed();
            }

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
            return await _context.Users.Include(c => c.Company).FirstOrDefaultAsync(u => user.Id == u.Id);
        }

        internal async Task<IdentityResult> UpdateUserAndCompanyDataAsync(User user, ProfileViewModel model)
        {
       
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                user.Email = model.Email;
                user.PhoneNumber = model.Phone;
                                              
                user.UserName =  model.Email; 

                if (user.Company != null)
                {
                    user.Company.CompanyName = model.CompanyName;
                    user.Company.Inn = model.Inn;
                    user.Company.Kpp = model.Kpp;
                    user.Company.LegalAddress = model.Address;
                    user.Company.ContactPerson =model.ContactPerson;
                }

                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    await transaction.CommitAsync();
                    return IdentityResult.Success;
                }
                await transaction.RollbackAsync();
                return IdentityResult.Failed();
            }
            catch
            {
                await transaction.RollbackAsync();
                return IdentityResult.Failed();
            }
        }

        internal async Task<IdentityResult> UpdateSecurityData(User user, ProfileViewModel model)
        {
            var res = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword) ;
            return res;
        }
    } }

