using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LitShare.BLL.Services;

namespace LitShare.Presentation
{
    public partial class AuthWindow : Window
    {
        private readonly UserService _userService = new UserService();

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            ClearValidation(); // очистимо старі помилки

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string region = txtRegion.Text.Trim();
            string district = txtDistrict.Text.Trim();
            string city = txtCity.Text.Trim();

            bool hasError = false;

            if (string.IsNullOrEmpty(name))
            {
                ShowError(txtName, errorName, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowError(txtEmail, errorEmail, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (!IsValidEmail(email))
            {
                ShowError(txtEmail, errorEmail, "Некоректний формат E-mail.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(phone))
            {
                ShowError(txtPhone, errorPhone, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (!IsValidPhone(phone))
            {
                ShowError(txtPhone, errorPhone, "Номер телефону має бути у форматі +380XXXXXXXXX або 0XXXXXXXXX.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError(txtPassword, errorPassword, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (!IsValidPassword(password))
            {
                ShowError(txtPassword, errorPassword, "Пароль має бути не менше 8 символів.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                ShowError(txtConfirmPassword, errorConfirmPassword, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }
            else if (password != confirmPassword)
            {
                ShowError(txtConfirmPassword, errorConfirmPassword, "Паролі не збігаються.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(region))
            {
                ShowError(txtRegion, errorRegion, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(district))
            {
                ShowError(txtDistrict, errorDistrict, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (string.IsNullOrEmpty(city))
            {
                ShowError(txtCity, errorCity, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            if (hasError)
                return;

            try
            {
                _userService.AddUser(name, email, phone, password, region, district, city);
                mainTabs.SelectedItem = loginTab;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка реєстрації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            ClearLoginValidation(); // очистимо старі помилки

            var button = sender as Button;
            button.IsEnabled = false;

            try
            {
                string email = txtLoginEmail.Text.Trim();
                string password = txtLoginPassword.Password;

                bool hasError = false;

                // Валідація електронної пошти
                if (string.IsNullOrEmpty(email))
                {
                    ShowError(txtLoginEmail, errorLoginEmail, "Це поле обов'язкове для заповнення.");
                    hasError = true;
                }
                else if (!IsValidEmail(email))
                {
                    ShowError(txtLoginEmail, errorLoginEmail, "Некоректний формат E-mail.");
                    hasError = true;
                }

                // Валідація пароля
                if (string.IsNullOrEmpty(password))
                {
                    ShowError(txtLoginPassword, errorLoginPassword, "Це поле обов'язкове для заповнення.");
                    hasError = true;
                }
                else if (!IsValidPassword(password))
                {
                    ShowError(txtLoginPassword, errorLoginPassword, "Пароль має бути не менше 8 символів.");
                    hasError = true;
                }

                if (hasError)
                {
                    button.IsEnabled = true;
                    return;
                }

                bool isValid = await _userService.ValidateUser(email, password);
                if (isValid)
                {
                    var user = _userService.GetAllUsers().FirstOrDefault(u => u.email == email);
                    if (user != null)
                    {
                        var mainPage = new MainPage(user.id);
                        mainPage.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Користувача не знайдено після входу.");
                    }
                }
                else
                {
                    ShowError(txtLoginEmail, errorLoginEmail, "Невірна пошта або пароль.");
                    ShowError(txtLoginPassword, errorLoginPassword, "Невірна пошта або пароль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        // 🧩 Валідація полів
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            string phonePattern = @"^(\+380\d{9}|0\d{9})$";
            return Regex.IsMatch(phone, phonePattern);
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            return password.Length >= 8;
        }

        // 🧠 Допоміжні методи
        private void ShowError(Control control, TextBlock errorBlock, string message)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(1.5);
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
        }

        private void ClearValidation()
        {
            ResetField(txtName, errorName);
            ResetField(txtEmail, errorEmail);
            ResetField(txtPhone, errorPhone);
            ResetField(txtPassword, errorPassword);
            ResetField(txtConfirmPassword, errorConfirmPassword);
            ResetField(txtRegion, errorRegion);
            ResetField(txtDistrict, errorDistrict);
            ResetField(txtCity, errorCity);
        }

        private void ClearLoginValidation()
        {
            ResetField(txtLoginEmail, errorLoginEmail);
            ResetField(txtLoginPassword, errorLoginPassword);
        }

        private void ResetField(Control control, TextBlock errorBlock)
        {
            control.BorderBrush = Brushes.Gray;
            control.BorderThickness = new Thickness(1);
            errorBlock.Text = string.Empty;
            errorBlock.Visibility = Visibility.Collapsed;
        }
    }
}