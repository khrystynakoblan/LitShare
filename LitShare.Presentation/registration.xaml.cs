namespace LitShare.Presentation
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using LitShare.BLL.Services;

    /// <summary>
    /// Логіка взаємодії для AuthWindow.xaml.
    /// Вікно для реєстрації та входу користувача.
    /// </summary>
    public partial class AuthWindow : Window
    {
        private readonly UserService userService = new UserService();

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="AuthWindow"/>.
        /// </summary>
        public AuthWindow()
        {
            this.InitializeComponent();
        }

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

            // Валідація імені
            if (string.IsNullOrEmpty(name))
            {
                this.ShowError(this.txtName, this.errorName, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            // Валідація E-mail
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

            // Валідація телефону
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

            // Валідація пароля
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

            // Валідація підтвердження пароля
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

            // Валідація регіону
            if (string.IsNullOrEmpty(region))
            {
                this.ShowError(this.txtRegion, this.errorRegion, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            // Валідація району
            if (string.IsNullOrEmpty(district))
            {
                this.ShowError(this.txtDistrict, this.errorDistrict, "Це поле обов'язкове для заповнення.");
                hasError = true;
            }

            // Валідація міста
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
                MessageBox.Show($"Помилка реєстрації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

                    if (user != null)
                    {
                        var mainPage = new MainPage(user.Id);
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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            string phonePattern = @"^(\+380\d{9}|0\d{9})$";
            return Regex.IsMatch(phone, phonePattern);
        }

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

        private void ShowError(Control control, TextBlock errorBlock, string message)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(1.5);
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
        }

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

        private void ClearLoginValidation()
        {
            this.ResetField(this.txtLoginEmail, this.errorLoginEmail);
            this.ResetField(this.txtLoginPassword, this.errorLoginPassword);
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