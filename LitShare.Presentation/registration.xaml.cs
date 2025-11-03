using System; // Потрібно для Exception
using System.Windows;
using System.Windows.Controls;
using LitShare.BLL.Services; // 1. Підключаємо наш BLL

namespace LitShare.Presentation
{
    public partial class AuthWindow : Window
    {
        // 2. Створюємо екземпляр сервісу
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

            // --- ТУТ ПОЧАТОК ЄДИНОГО ПРАВИЛЬНОГО БЛОКУ 'TRY...CATCH' ---
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
                // --- ЦЕ ОНОВЛЕНИЙ CATCH, ЯКИЙ "РОЗПАКОВУЄ" ПОМИЛКУ ---

                string errorMessage = ex.Message;

                // Ця частина "розпаковує" справжню помилку з Supabase
                if (ex.InnerException != null)
                {
                    Exception innerEx = ex.InnerException;
                    // Ми "провалюємось" вглиб, поки не знайдемо корисне повідомлення
                    while (innerEx.InnerException != null)
                    {
                        innerEx = innerEx.InnerException;
                    }
                    errorMessage = innerEx.Message; // Беремо найглибше повідомлення
                }

                // Показуємо його
                MessageBox.Show($"Під час реєстрації сталася помилка: \n\n{errorMessage}",
                                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // --- ТУТ КІНЕЦЬ ЄДИНОГО БЛОКУ 'TRY...CATCH' ---
        }

        // 6. Заготовка для кнопки "Увійти"
        private async void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            // Отримуємо посилання на кнопку, яка викликала подію
            var button = sender as Button;
            button.IsEnabled = false;

            try
            {
                bool isValid = await _userService.ValidateUser(
                    txtLoginEmail.Text,
                    txtLoginPassword.Password
                );

                if (isValid)
                {
                    MessageBox.Show("Вхід успішний!");
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