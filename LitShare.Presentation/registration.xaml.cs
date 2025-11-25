// <copyright file="registration.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;

    /// <summary>
    /// Interaction logic for AuthWindow.xaml. This window handles user registration and login.
    /// </summary>
    public partial class AuthWindow : Window
    {
        // SA1309: Private fields should not begin with an underscore.
        private readonly UserService userService = new UserService();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthWindow"/> class.
        /// </summary>
        public AuthWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the click event for the registration button. Validates user input and attempts to register a new user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            this.ClearValidation();

            string name = this.txtName.Text.Trim();
            string email = this.txtEmail.Text.Trim();
            string phone = this.txtPhone.Text.Trim();
            string password = this.txtPassword.Password;
            string confirmPassword = this.txtConfirmPassword.Password;
            string region = this.txtRegion.Text.Trim();
            string district = this.txtDistrict.Text.Trim();
            string city = this.txtCity.Text.Trim();

            bool hasError = false;

            if (string.IsNullOrEmpty(name))
            {
                this.ShowError(this.txtName, this.errorName, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(email))
            {
                this.ShowError(this.txtEmail, this.errorEmail, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (!this.IsValidEmail(email))
            {
                this.ShowError(this.txtEmail, this.errorEmail, "Некоректний формат E-mail.");
                hasError = true;
            }
            else if (this.IsEmailExists(email))
            {
                this.ShowError(this.txtEmail, this.errorEmail, "Користувач з такою поштою вже зареєстрований.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(phone))
            {
                this.ShowError(this.txtPhone, this.errorPhone, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (!this.IsValidPhone(phone))
            {
                this.ShowError(this.txtPhone, this.errorPhone, "Має бути у форматі +380XXXXXXXXX або 0XXXXXXXXX.");
                hasError = true;
            }
            else if (this.IsPhoneExists(phone))
            {
                this.ShowError(this.txtPhone, this.errorPhone, "Користувач з таким номером вже зареєстрований.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(password))
            {
                this.ShowError(this.txtPassword, this.errorPassword, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (!this.IsValidPassword(password, out string passwordError))
            {
                this.ShowError(this.txtPassword, this.errorPassword, passwordError);
                hasError = true;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                this.ShowError(this.txtConfirmPassword, this.errorConfirmPassword, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (password != confirmPassword)
            {
                this.ShowError(this.txtConfirmPassword, this.errorConfirmPassword, "Паролі не збігаються.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(region))
            {
                this.ShowError(this.txtRegion, this.errorRegion, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(district))
            {
                this.ShowError(this.txtDistrict, this.errorDistrict, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(city))
            {
                this.ShowError(this.txtCity, this.errorCity, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            try
            {
                this.userService.AddUser(name, email, phone, password, region, district, city);
                this.mainTabs.SelectedItem = this.loginTab;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(
                    $"Помилка реєстрації: {ex.Message}\n\n{ex.InnerException?.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event for the login button. Validates user input and attempts to log in the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            this.ClearLoginValidation(); // очистимо старі помилки

            var button = sender as Button;
            if (button != null)
            {
                button.IsEnabled = false;
            }

            try
            {
                string email = this.txtLoginEmail.Text.Trim();
                string password = this.txtLoginPassword.Password;

                bool hasError = false;

                // Валідація електронної пошти
                if (string.IsNullOrEmpty(email))
                {
                    this.ShowError(this.txtLoginEmail, this.errorLoginEmail, "Це поле обов'язкове для заповнення.");
                    hasError = true;
                }
                else if (!this.IsValidEmail(email))
                {
                    this.ShowError(this.txtLoginEmail, this.errorLoginEmail, "Некоректний формат E-mail.");
                    hasError = true;
                }

                // Валідація пароля
                if (string.IsNullOrEmpty(password))
                {
                    this.ShowError(this.txtLoginPassword, this.errorLoginPassword, "Це поле обов'язкове для заповнення.");
                    hasError = true;
                }
                else if (password.Length < 8)
                {
                    this.ShowError(this.txtLoginPassword, this.errorLoginPassword, "Пароль має бути не менше 8 символів.");
                    hasError = true;
                }

                if (hasError)
                {
                    if (button != null)
                    {
                        button.IsEnabled = true;
                    }

                    return;
                }

                bool isValid = await this.userService.ValidateUser(email, password);
                if (isValid)
                {
                    var user = this.userService.GetAllUsers().FirstOrDefault(u => u.Email == email);

                    if (user == null)
                    {
                        MessageBox.Show("Користувача не знайдено.");
                        return;
                    }

                    if (user.Role == RoleType.Admin)
                    {
                        var adminWindow = new ComplaintsPage();
                        adminWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        var mainPage = new MainPage(user.Id);
                        mainPage.Show();
                        this.Close();
                    }
                }
                else
                {
                    this.ShowError(this.txtLoginEmail, this.errorLoginEmail, "Невірна пошта або пароль.");
                    this.ShowError(this.txtLoginPassword, this.errorLoginPassword, "Невірна пошта або пароль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
            finally
            {
                if (button != null)
                {
                    button.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Checks if the provided string is a valid email format.
        /// </summary>
        /// <param name="email">The email string to validate.</param>
        /// <returns>True if the email is valid, false otherwise.</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        /// <summary>
        /// Checks if the provided string is a valid Ukrainian phone number format (+380XXXXXXXXX or 0XXXXXXXXX).
        /// </summary>
        /// <param name="phone">The phone number string to validate.</param>
        /// <returns>True if the phone number is valid, false otherwise.</returns>
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            string phonePattern = @"^(\+380\d{9}|0\d{9})$";
            return Regex.IsMatch(phone, phonePattern);
        }

        /// <summary>
        /// Checks if the provided password meets security requirements (length, case, digit).
        /// </summary>
        /// <param name="password">The password string to validate.</param>
        /// <param name="errorMessage">Output parameter for the specific error message if validation fails.</param>
        /// <returns>True if the password is valid, false otherwise.</returns>
        private bool IsValidPassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Пароль не може бути порожнім.";
                return false;
            }

            if (password.Length < 8)
            {
                errorMessage = "Пароль має містити мінімум 8 символів.";
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                errorMessage = "Пароль має містити хоча б одну цифру.";
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                errorMessage = "Пароль має містити хоча б одну велику літеру.";
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                errorMessage = "Пароль має містити хоча б одну малу літеру.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the database to see if a user with the given email already exists.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>True if the email exists, false otherwise.</returns>
        private bool IsEmailExists(string email)
        {
            try
            {
                var users = this.userService.GetAllUsers();
                return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the database to see if a user with the given phone number already exists.
        /// </summary>
        /// <param name="phone">The phone number to check.</param>
        /// <returns>True if the phone number exists, false otherwise.</returns>
        private bool IsPhoneExists(string phone)
        {
            try
            {
                var users = this.userService.GetAllUsers();
                return users.Any(u => u.Phone == phone);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Displays an error message and highlights the corresponding control in red.
        /// </summary>
        /// <param name="control">The control to highlight.</param>
        /// <param name="errorBlock">The TextBlock to display the error message in.</param>
        /// <param name="message">The error message.</param>
        private void ShowError(Control control, TextBlock errorBlock, string message)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(1.5);
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clears validation errors and resets the appearance for all registration fields.
        /// </summary>
        private void ClearValidation()
        {
            this.ResetField(this.txtName, this.errorName);
            this.ResetField(this.txtEmail, this.errorEmail);
            this.ResetField(this.txtPhone, this.errorPhone);
            this.ResetField(this.txtPassword, this.errorPassword);
            this.ResetField(this.txtConfirmPassword, this.errorConfirmPassword);
            this.ResetField(this.txtRegion, this.errorRegion);
            this.ResetField(this.txtDistrict, this.errorDistrict);
            this.ResetField(this.txtCity, this.errorCity);
        }

        /// <summary>
        /// Clears validation errors and resets the appearance for all login fields.
        /// </summary>
        private void ClearLoginValidation()
        {
            this.ResetField(this.txtLoginEmail, this.errorLoginEmail);
            this.ResetField(this.txtLoginPassword, this.errorLoginPassword);
        }

        /// <summary>
        /// Resets the appearance of a control and clears its associated error message.
        /// </summary>
        /// <param name="control">The control to reset.</param>
        /// <param name="errorBlock">The TextBlock associated with the error.</param>
        private void ResetField(Control control, TextBlock errorBlock)
        {
            control.BorderBrush = Brushes.Gray;
            control.BorderThickness = new Thickness(1);
            errorBlock.Text = string.Empty;
            errorBlock.Visibility = Visibility.Collapsed;
        }
    }
}