using System;
using System.Windows;
using System.Windows.Controls;
using LitShare.BLL.Services; // 1. Підключаємо наш BLL

namespace LitShare.Presentation
{
    public partial class AuthWindow : Window
    {
        private readonly UserService _userService = new UserService();

        public AuthWindow()
        {
            InitializeComponent();
        }

        // --- ОНОВЛЕНИЙ МЕТОД РЕЄСТРАЦІЇ ---
        private void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            // Збираємо дані з усіх полів
            string name = txtName.Text;
            string email = txtEmail.Text;

            // 1. ЗЧИТУЄМО НОВЕ ПОЛЕ
            string phone = txtPhone.Text;

            string password = txtPassword.Password;
            string region = txtRegion.Text;
            string district = txtDistrict.Text;
            string city = txtCity.Text;

            // 2. ОНОВЛЮЄМО ВАЛІДАЦІЮ
            // (Додаємо перевірку на телефон)
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Будь ласка, заповніть Ім'я, Пошту, Номер телефону та Пароль.",
                                "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 3. ОНОВЛЮЄМО ВИКЛИК МЕТОДУ BLL
                // (Передаємо 'phone' у сервіс)
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


        // 6. Заготовка для кнопки "Увійти"
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
                    // ✅ Отримуємо користувача з БД
                    var user = _userService.GetAllUsers()
                        .FirstOrDefault(u => u.email == email);

                    if (user != null)
                    {
                        MessageBox.Show($"Вхід успішний! Вітаємо, {user.name}.");

                        // ✅ Передаємо айді користувача в MainPage
                        var mainPage = new MainPage(user.id);
                        mainPage.Show();

                        this.Close(); // закриваємо вікно авторизації
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

    }
}