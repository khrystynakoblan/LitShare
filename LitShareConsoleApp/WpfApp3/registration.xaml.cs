using System.Windows;
using LitShare.BLL.Services; // 1. Підключаємо наш BLL

namespace LitShare.Presentation
{
    public partial class AuthWindow : Window
    {
        // 2. Створюємо екземпляр сервісу, який ми оновили
        private readonly UserService _userService = new UserService();

        public AuthWindow()
        {
            InitializeComponent();
        }

        // 3. Цей метод спрацює при натисканні на кнопку "Створити акаунт"
        private void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            // Збираємо дані з усіх полів
            string name = txtName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password; // Для PasswordBox використовуємо .Password
            string region = txtRegion.Text;
            string district = txtDistrict.Text;
            string city = txtCity.Text;

            // Проста перевірка (валідація), що поля не порожні
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, заповніть Ім'я, Пошту та Пароль.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Не продовжуємо, якщо дані невірні
            }

            try
            {
                // 4. Викликаємо наш оновлений метод з BLL
                _userService.AddUser(name, email, password, region, district, city);

                // Якщо все пройшло успішно
                MessageBox.Show("Акаунт успішно створено! Тепер ви можете увійти.", "Реєстрація успішна", MessageBoxButton.OK, MessageBoxImage.Information);

                // 5. Автоматично перекидаємо користувача на вкладку "Увійти"
                mainTabs.SelectedItem = loginTab;
            }
            catch (Exception ex)
            {
                // Обробляємо будь-які помилки (наприклад, такий email вже існує)
                MessageBox.Show($"Під час реєстрації сталася помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 6. Заготовка для кнопки "Увійти"
        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            string email = txtLoginEmail.Text;
            string password = txtLoginPassword.Password;

            // TODO: Додайте логіку для перевірки логіна та пароля
            MessageBox.Show("Логіка входу ще не реалізована.", "В розробці", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}