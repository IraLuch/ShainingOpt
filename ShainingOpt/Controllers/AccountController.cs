using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        public IActionResult Profile()
        {
            return View();
        }

        // [HttpPost]
        // [Authorize]
        // public IActionResult Profile()
        // {
        //     return View();
        // }
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

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

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
            //var decodeToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
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

    }
}
