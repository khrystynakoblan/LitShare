namespace LitShare.Web.Controllers
{
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Models;
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
                this.logger.LogWarning(
                    "Registration form validation failed. Email: {Email}",
                    model.Email);
                return this.View(model);
            }

            this.logger.LogInformation(
                "Registration POST request received. Email: {Email}",
                model.Email);

            try
            {
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

                bool success = await this.registerService.RegisterAsync(dto);

                if (!success)
                {
                    this.logger.LogWarning(
                        "Registration failed: email {Email} is already taken.",
                        model.Email);

                    this.ModelState.AddModelError(
                        nameof(model.Email),
                        "Цей email вже зареєстрований у системі.");

                    return this.View(model);
                }

                this.logger.LogInformation(
                    "Registration successful. Redirecting to Login. Email: {Email}",
                    model.Email);

                this.TempData["SuccessMessage"] = "Реєстрацію успішно завершено! Будь ласка, увійдіть.";
                return this.RedirectToAction(nameof(this.Login));
            }
            catch (ArgumentException ex)
            {
                this.logger.LogWarning(
                    "Service validation error during registration. Email: {Email}",
                    model.Email);

                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.View(model);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Unexpected error during registration. Email: {Email}",
                    model.Email);

                this.ModelState.AddModelError(
                    string.Empty,
                    "Виникла непередбачена помилка. Спробуйте пізніше.");

                return this.View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.HttpContext.Session.GetString("UserEmail") != null)
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
                this.logger.LogWarning(
                    "Login form validation failed. Email: {Email}",
                    model.Email);
                return this.View(model);
            }

            this.logger.LogInformation(
                "Login POST request received. Email: {Email}",
                model.Email);

            try
            {
                var dto = new LoginDto
                {
                    Email = model.Email,
                    Password = model.Password,
                };

                bool success = await this.loginService.LoginAsync(dto);

                if (!success)
                {
                    this.logger.LogWarning(
                        "Login failed: invalid credentials. Email: {Email}",
                        model.Email);

                    this.ModelState.AddModelError(
                        string.Empty,
                        "Невірний email або пароль.");

                    return this.View(model);
                }

                this.HttpContext.Session.SetString("UserEmail", model.Email);

                this.logger.LogInformation(
                    "Login successful. Redirecting to Home. Email: {Email}",
                    model.Email);

                return this.RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                this.logger.LogWarning(
                    "Service validation error during login. Email: {Email}",
                    model.Email);

                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.View(model);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Unexpected error during login. Email: {Email}",
                    model.Email);

                this.ModelState.AddModelError(
                    string.Empty,
                    "Виникла непередбачена помилка. Спробуйте пізніше.");

                return this.View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            this.logger.LogInformation(
                "User logged out. Email: {Email}",
                this.HttpContext.Session.GetString("UserEmail"));

            this.HttpContext.Session.Clear();
            return this.RedirectToAction(nameof(this.Login));
        }
    }
}