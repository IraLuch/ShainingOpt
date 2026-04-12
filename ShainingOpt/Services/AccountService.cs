using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.ViewModels;

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
            //var encodedToken = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(token));
            return token;
        }

        internal async Task<IdentityResult> ResetPasswordAsync(User user, NewPasswordViewModel model)
        {
            var res = await _userManager.ResetPasswordAsync(user, model.Token ,model.NewPassword);
            return res;

        }
    } }

