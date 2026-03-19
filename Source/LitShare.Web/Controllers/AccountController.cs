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
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IRegisterService registerService,
            ILogger<AccountController> logger)
        {
            this.registerService = registerService;
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
            return this.View();
        }
    }
}