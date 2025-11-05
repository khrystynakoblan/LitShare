using System;
using System.Windows;
using System.Windows.Controls;
using LitShare.BLL.Services;
using System.Text.RegularExpressions;
using System.Linq;
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
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Password; 
            string region = txtRegion.Text.Trim();
            string district = txtDistrict.Text.Trim();
            string city = txtCity.Text.Trim();

            var errors = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(name))
            {
                errors.AppendLine("- Ім'я не може бути порожнім.");
            }

            if (!IsValidEmail(email))
            {
                errors.AppendLine("- Введено некоректний формат E-mail.");
            }

            if (!IsValidPhone(phone))
            {
                errors.AppendLine("- Номер телефону має бути у форматі +380XXXXXXXXX або 0XXXXXXXXX.");
            }

            if (!IsValidPassword(password))
            {
                errors.AppendLine("- Пароль має бути не менше 8 символів.");
            }


            if (errors.Length > 0)
            {
                MessageBox.Show(
                    "Будь ласка, виправте наступні помилки:\n\n" + errors.ToString(),
                    "Помилка валідації",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                _userService.AddUser(name, email, phone, password, region, district, city);

                MessageBox.Show("Акаунт успішно створено! Тепер ви можете увійти.",
                                "Реєстрація успішна", MessageBoxButton.OK, MessageBoxImage.Information);

                mainTabs.SelectedItem = loginTab;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    Exception innerEx = ex.InnerException;
                    while (innerEx.InnerException != null)
                    {
                        innerEx = innerEx.InnerException;
                    }
                    errorMessage = innerEx.Message;
                }

                MessageBox.Show($"Під час реєстрації сталася помилка: \n\n{errorMessage}",
                                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;

            try
            {
                string email = txtLoginEmail.Text;
                string password = txtLoginPassword.Password;

                bool isValid = await _userService.ValidateUser(email, password);
                if (isValid)
                {
                    var user = _userService.GetAllUsers()
                        .FirstOrDefault(u => u.email == email);

                    if (user != null)
                    {
                        MessageBox.Show($"Вхід успішний! Вітаємо, {user.name}.");

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
                    MessageBox.Show("Невірна пошта або пароль.");
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

            if (password.Length < 8) return false;

            return true;
        }
    }
}