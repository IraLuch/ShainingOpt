using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShainingOpt.Mappers;
using ShainingOpt.Services;
using ShainingOpt.ViewModels.Account;

namespace ShainingOpt.Areas_Manager_Controllers

{
    /// <summary>
    /// Контроллер для управления учетными записями 
    /// </summary>
    /// 

    [Area("Manager")]
    public class AccountController : Controller
    {
      
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
         
        }


        /// <summary>
        /// Выход из системы
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

    }
}
