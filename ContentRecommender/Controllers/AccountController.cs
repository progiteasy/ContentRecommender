using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentRecommender.Data.Models;
using ContentRecommender.Extensions;
using ContentRecommender.ViewModels.Account;

namespace ContentRecommender.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private async Task<IActionResult> GetViewAsync(string name)
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (loggedInUser == null || !loggedInUser.IsActive)
                    return View(name);

                return Redirect("~/");
            }

            return View(name);
        }

        [HttpGet("~/account/register")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegistrationViewAsync()
            => await GetViewAsync("RegistrationView");

        [HttpPost("~/account/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegistrationViewModel registrationModel)
        {
            if (!ModelState.IsValid)
                return View("RegistrationView", registrationModel);

            var errorsToCheck = new Dictionary<string, bool>()
            {
                ["An account with this email address already exists"] = await _userManager.FindByEmailAsync(registrationModel.Email) != null,
                ["An account with this username already exists"] = await _userManager.FindByNameAsync(registrationModel.UserName) != null
            };
            if (ModelState.TryAddError(errorsToCheck))
                return View("RegistrationView", registrationModel);

            var user = new User()
            {
                UserName = registrationModel.UserName,
                Email = registrationModel.Email,
                RegistrationDate = DateTime.Now,
                IsActive = true
            };
            var userCreationResult = await _userManager.CreateAsync(user, registrationModel.Password);

            errorsToCheck = new Dictionary<string, bool>()
            {
                ["The username contains invalid characters"] = userCreationResult.Succeeded == false,
            };
            if (ModelState.TryAddError(errorsToCheck))
                return View("RegistrationView", registrationModel);

            return Redirect("~/account/login");
        }

        [HttpGet("~/account/login")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginViewAsync()
            => await GetViewAsync("LoginView");

        [HttpPost("~/account/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogInAsync(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View("LoginView", loginModel);

            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            var userLoginResult = Microsoft.AspNetCore.Identity.SignInResult.Failed;
            
            if (user != null)
                userLoginResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, true, false);

            var errorsToCheck = new Dictionary<string, bool>()
            {
                ["The email address or password is incorrect"] = userLoginResult.Succeeded == false,
                ["The account is blocked in the system"] = user?.IsActive == false
            };
            if (ModelState.TryAddError(errorsToCheck))
                return View("LoginView", loginModel);

            return Redirect("~/");
        }

        [HttpPost("~/account/logout")]
        [Authorize]
        public async Task<IActionResult> LogOutAsync()
        {
            await _signInManager.SignOutAsync();

            return Redirect("~/");
        }
    }
}
