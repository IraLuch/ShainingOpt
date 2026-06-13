using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ShainingOpt.Areas_Admin_Controllers;
using ShainingOpt.Controllers;
using ShainingOpt.Models;
using ShainingOpt.Services.Interfaces;
using ShainingOpt.ViewModels.Account;
using ShainingOpt.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ShainingOpt.UnitTests.Controller
{
    public class AccountControllerTest
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly ICartService _cartService;
        private readonly Controllers.AccountController _accountController;

        public AccountControllerTest()
        {
            _accountService = A.Fake<IAccountService>();
            _emailService = A.Fake<IEmailService>();  
            _cartService = A.Fake<ICartService>();
            _accountController = new Controllers.AccountController(_accountService, _emailService, _cartService);
        }

        [Fact]
        public async Task AccountControllerTest_Register_WhenModelIsNotValid()
        {
            var viewModel = new RegisterViewModel();
            _accountController.ModelState.AddModelError("Inn", "Обязательное поле");

            var result = await _accountController.Register(viewModel);

            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<RegisterViewModel>();

        }

        [Fact]
        public async Task AccountControllerTest_Register_WnenNotSucceeded()
        {
            var viewModel = new RegisterViewModel()
            {
               
                Email = "user@test.com",
                PhoneNumber = "89647390003",
                  CompanyName = "Тест название",
                    Inn = "1234567890",
                    Kpp = "123456789",
                    LegalAddress = "Тест адрес",
                    ContactPerson = "Иванов Иван Иванович"

                
            };

            A.CallTo(() => _accountService.RegisterUserAsync(viewModel)).Returns(
                 IdentityResult.Failed(
            new IdentityError
            {
                Description = "Пользователь уже существует"
            }));

            var result = await _accountController.Register(viewModel);

            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<RegisterViewModel>();

        }

        [Fact]
        public async Task AccountControllerTest_Register_WnenSucceeded()
        {
            var viewModel = new RegisterViewModel()
            {

                Email = "user@test.com",
                PhoneNumber = "89647390003",
                CompanyName = "Тест название",
                Inn = "1234567890",
                Kpp = "123456789",
                LegalAddress = "Тест адрес",
                ContactPerson = "Иванов Иван Иванович"


            };


            A.CallTo(() => _accountService.RegisterUserAsync(viewModel)).Returns(IdentityResult.Success);

            var result = await _accountController.Register(viewModel);

            result.Should().BeOfType<RedirectToActionResult>();

            var viewResult = result as RedirectToActionResult;
            viewResult.Should().NotBeNull();
            viewResult.ActionName.Should().Be("Profile");
            viewResult.ControllerName.Should().Be("Account");

        }

        [Fact]
        public async Task AccountControllerTest_Login_WhenModelIsNotValid()
        {
            var viewModel = new LoginViewModel();
            _accountController.ModelState.AddModelError("Email", "Обязательное поле");

            var result = await _accountController.Login(viewModel);

            result.Should().BeOfType<OkObjectResult>();
            var json = result as OkObjectResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new { success = false, message = "Заполните все поля корректно" });
        }


        [Fact]
        public async Task AccountControllerTest_Login_WhenNotSucceeded()
        {
            var viewModel = new LoginViewModel();

            A.CallTo(() => _accountService.LoginUserAsync(viewModel)).Returns(
                    IdentityResult.Failed(
               new IdentityError
               {
                   Description = "Пользователь уже существует"
               }));

            var result = await _accountController.Login(viewModel);

            result.Should().BeOfType<OkObjectResult>();
            var json = result as OkObjectResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new { success = false, message = "Неверный логин или пароль" });
        }


        [Fact]
        public async Task AccountControllerTest_Login_ReturnAdminProfile()
        {
            var viewModel = new LoginViewModel() { Email = "test@mail.ru", Password = "123!user"};

            var user = new User { Id = 10 };


            A.CallTo(() => _accountService.LoginUserAsync(viewModel)).Returns(
                    IdentityResult.Success);
            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(Task.FromResult(user));
 
            A.CallTo(() => _accountService.IsInRoleAsync(user, "Admin"))
       .Returns(true);


            var result = await _accountController.Login(viewModel);

            result.Should().BeOfType<OkObjectResult>();
            var json = result as OkObjectResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new
            {
                success = true,
                redirectUrl = "/Admin/Products"
            });

        }

        [Fact]
        public async Task AccountControllerTest_Login_ReturnManagerProfile()
        {
            var viewModel = new LoginViewModel() { Email = "test@mail.ru", Password = "123!user" };

            var user = new User { Id = 10 };


            A.CallTo(() => _accountService.LoginUserAsync(viewModel)).Returns(
                    IdentityResult.Success);
            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(Task.FromResult(user));


            A.CallTo(() => _accountService.IsInRoleAsync(user, "Manager"))
       .Returns(true);


            var result = await _accountController.Login(viewModel);

            result.Should().BeOfType<OkObjectResult>();
            var json = result as OkObjectResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new
            {
                success = true,
                redirectUrl = "/Manager/Products"
            });

        }


        [Fact]
        public async Task AccountControllerTest_Login_ReturnUserProfile()
        {
            var viewModel = new LoginViewModel() { Email = "test@mail.ru", Password = "123!user" };

            var user = new User { Id = 10 };


            A.CallTo(() => _accountService.LoginUserAsync(viewModel)).Returns(
                    IdentityResult.Success);
            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(Task.FromResult(user));

            A.CallTo(() => _accountService.IsInRoleAsync(user, "User"))
       .Returns(true);


            A.CallTo(() => _cartService.MergeCarts(Guid.NewGuid().ToString(), 1)).Returns(Task.CompletedTask);

            var result = await _accountController.Login(viewModel);

            result.Should().BeOfType<OkObjectResult>();
            var json = result as OkObjectResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new
            {
                success = true,
                redirectUrl = "/Account/Profile"
            });

        }

        [Fact]
        public async Task AccountControllerTest_ResetPassword_WhenEmailIncorrect()
        {
            var result = await _accountController.ResetPassword("");

            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("ResetPassword");

        }
        [Fact]
        public async Task AccountControllerTest_ResetPassword_WhenUserNotFound()
        {
            var email = "test@mail.ru";
            A.CallTo(() => _accountService.UserExsiting(email)).Returns((User?)null);
            var result = await _accountController.ResetPassword(email);

            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("ResetPassword");
        }

        [Fact]
        public async Task AccountControllerTest_ResetPassword_WhereEmailisSend()
        {
            var user = new User();
            var email = "test@mail.ru";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var urlHelper = A.Fake<IUrlHelper>();
            A.CallTo(() => urlHelper.Action(A<UrlActionContext>._)).Returns("http://fake-link.com");
            _accountController.Url = urlHelper;
            A.CallTo(() => _accountService.UserExsiting(email)).Returns(user);

            A.CallTo(() => _accountService.GetTokenAsync(user)).Returns("fake-token");
            A.CallTo(() => _emailService.SendEmailAsync(email, A<string>._, A<string>._)).Returns(Task.CompletedTask);

            var result = await _accountController.ResetPassword(email);

            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("ResetPassword");
        }

        [Fact]
        public async Task AccountControllerTest_NewPassword_WhenModelIsNotValid()
        {
            var viewModel = new NewPasswordViewModel();
            _accountController.ModelState.AddModelError("Email", "Обязательное поле");

            var result = await _accountController.NewPassword(viewModel);

            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<NewPasswordViewModel>();
        }

        [Fact]
        public async Task AccountControllerTest_NewPassword_WhenNotSucceeded()
        {
            var viewModel = new NewPasswordViewModel() { Email = "test@mail.ru", Token = "fake-token", NewPassword = "123!User", ConfirmPassword = "123!User" };
            var user = new User();

            A.CallTo(() => _accountService.UserExsiting(viewModel.Email)).Returns(user);
            A.CallTo(() => _accountService.ResetPasswordAsync(user, viewModel)).Returns(
                    IdentityResult.Failed(
               new IdentityError
               {
                   Description = "Ошибка при изменении"
               }));

            var result = await _accountController.NewPassword(viewModel);
            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<NewPasswordViewModel>();
        }

        [Fact]
        public async Task AccountControllerTest_NewPassword()
        {
            var viewModel = new NewPasswordViewModel() { Email = "test@mail.ru", Token = "fake-token", NewPassword = "123!User", ConfirmPassword = "123!User" };
            var user = new User();

            A.CallTo(() => _accountService.UserExsiting(viewModel.Email)).Returns(user);
            A.CallTo(() => _accountService.ResetPasswordAsync(user, viewModel)).Returns(IdentityResult.Success);

            var result = await _accountController.NewPassword(viewModel);

            result.Should().BeOfType<RedirectToActionResult>();

            var viewResult = result as RedirectToActionResult;
            viewResult.Should().NotBeNull();
            viewResult.ActionName.Should().Be("Index");
            viewResult.ControllerName.Should().Be("Home");
        }
    }
}
