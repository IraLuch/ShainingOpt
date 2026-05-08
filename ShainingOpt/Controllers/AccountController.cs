using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ShainingOpt.Mappers;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.ViewModels;
using System.Text;

namespace ShainingOpt.Controllers
{

    public class AccountController : Controller
    {
      
        private readonly AccountService _accountService;
        private readonly EmailService _emailService;
        public AccountController(AccountService accountService, EmailService emailService)
        {
            _accountService = accountService;
            _emailService = emailService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {

            var user = await _accountService.GetCurrentUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Register", "Account");
            }
            if (user.Company is null)
            {
               
                return View(new ProfileViewModel { Email = user.Email, Phone = user.PhoneNumber });
            }
            var model = new ProfileViewModel
            {
                Id = user.Company.CompanyId,
                CompanyName = user.Company.CompanyName,
                Inn = user.Company.Inn,
                Kpp = user.Company.Kpp,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Company.LegalAddress,
                ContactPerson = user.Company.ContactPerson
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfileData(UpdateProfileDataViewModel model)
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Register", "Account"); // страница ошибки
            }
            var profileModel = ProfileMapper.FromUpdateModel(model, user);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                foreach (var er in errors)
                {
                    ModelState.AddModelError("ProfileError", er);
                }
                return View("Profile", profileModel);
            }
            var res = await _accountService.UpdateUserAndCompanyDataAsync(user, model);
            if (!res.Succeeded)
            {
                ModelState.AddModelError("ProfileError", "Что-то пошло не так. Попробуйте еще раз");

            }
            return View("Profile", profileModel);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateSecurityDate(UpdateSecurityDataViewModel model)
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user is null)
            {
                return RedirectToAction("Register", "Account"); // страница ошибки
            }
            var profileModel = ProfileMapper.ToViewModel(user);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                foreach (var er in errors)
                {
                    ModelState.AddModelError("SecurityError", er);
                }
                return View("Profile", profileModel);
            }
            var res = await _accountService.UpdateSecurityData(user, model);
            if (!res.Succeeded)
            {
                ModelState.AddModelError("SecurityError", "Что-то пошло не так. Попробуйте еще раз");

            }
            return View("Profile", profileModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var res = await _accountService.RegisterUserAsync(model) ;
           
            if (!res.Succeeded)
            {
                foreach (var er in res.Errors)
                {
                    ModelState.AddModelError("", er.Description);
                    return View(model);
                }
            }

            return RedirectToAction("Profile", "Account");
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { success = false, message = "Заполните все поля корректно" });
         
            }

            var res = await _accountService.LoginUserAsync(model);
            if (!res.Succeeded)
            {
                return Ok(new { success = false, message = "Неверный логин или пароль" });

            }

            return Ok(new { success = true });
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Введите почту");
                return View(nameof(ResetPassword));
            }
            var user = await _accountService.UserExsiting(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь с такой почтой не найден");
                return View(nameof(ResetPassword));
            }

            var token = await _accountService.GetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(NewPassword), "Account", new { email = email, token = token }, protocol: Request.Scheme);
            var message = $"Для сброса пароля <a href='{callbackUrl}'>нажмите здесь</a>.";
            var subject = "Сброс пароля";

            await _emailService.SendEmailAsync(email, subject, message );
            ModelState.AddModelError("", "Ссылка для сброса отправлена. Проверьте почту!");
            return View(nameof(ResetPassword));
        }

        [HttpGet] 
        public async Task<IActionResult> NewPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home"); //сделать страницу ошибки
            }
           
            var model = new NewPasswordViewModel() { Email = email, Token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword( NewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
           
            var user = await _accountService.UserExsiting(model.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var res = await _accountService.ResetPasswordAsync(user, model);
            if (!res.Succeeded)
            {
                foreach (var er in res.Errors)
                {
                    ModelState.AddModelError("", er.Description);
                }
                    return View(model);
            }

            return RedirectToAction("Index", "Home");


        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return RedirectToAction("Index", "Home");
        }

    }
}
