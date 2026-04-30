namespace LitShare.Web.Controllers
{
    using System.Security.Claims;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Models;
    using LitShare.Web.Filters;
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
        [RateLimit(MaxRequests = 1, WindowSeconds = 60)]
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
                if (this.User.IsInRole(RoleType.Admin.ToString()))
                {
                    return this.RedirectToAction("Index", "Admin");
                }

                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }

        [HttpPost]
        [RateLimit(MaxRequests = 3, WindowSeconds = 60)]
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

            var loginValue = loginResult.Value!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginValue.UserId.ToString()),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, loginValue.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            if (loginValue.Role == RoleType.Admin)
            {
                return this.RedirectToAction("Index", "Admin");
            }

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