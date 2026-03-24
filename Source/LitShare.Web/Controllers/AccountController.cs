namespace LitShare.Web.Controllers
{
    using System.Security.Claims;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class AccountController : Controller
    {
        private readonly IRegisterService registerService;
        private readonly ILoginService loginService;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IRegisterService registerService,
            ILoginService loginService,
            ILogger<AccountController> logger)
        {
            this.registerService = registerService;
            this.loginService = loginService;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            this.logger.LogInformation("User opened registration page.");
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var dto = new RegisterDto
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone,
                Region = model.Region,
                District = model.District,
                City = model.City,
            };

            var result = await this.registerService.RegisterAsync(dto);

            if (result.IsFailure)
            {
                this.ModelState.AddModelError(string.Empty, result.Error);
                return this.View(model);
            }

            this.TempData["SuccessMessage"] = "Реєстрацію успішно завершено! Будь ласка, увійдіть.";
            return this.RedirectToAction(nameof(this.Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.Identity != null && this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var dto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
            };

            var loginResult = await this.loginService.LoginAsync(dto);

            if (loginResult.IsFailure)
            {
                this.ModelState.AddModelError(string.Empty, loginResult.Error);
                return this.View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResult.Value.ToString()),
                new Claim(ClaimTypes.Email, model.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            this.logger.LogInformation("User logged out.");

            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToAction(nameof(this.Login));
        }
    }
}