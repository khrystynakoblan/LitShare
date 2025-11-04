using LitShare.BLL.Services; // 1. ДОДАНО: Підключаємо BLL
using System.Threading.Tasks;  // 2. ДОДАНО: Потрібно для асинхронності
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ProfileWindow : Window
    {
        // 3. ДОДАНО: Створюємо екземпляр сервісу
        private readonly UserService _userService = new UserService();

        public ProfileWindow()
        {
            InitializeComponent();

            // 4. ДОДАНО: Викликаємо завантаження даних
            int testUserId = 2; // <--- ВАШ ТЕСТОВИЙ ID (поставте ID, який існує в БД)
            _ = LoadUserProfileAsync(testUserId); // Запускаємо асинхронно
        }

        // 5. ДОДАНО: Новий метод для завантаження даних
        private async Task LoadUserProfileAsync(int userId)
        {
            try
            {
                // Отримуємо користувача. 
                // ВАЖЛИВО: Використовуємо GetUserProfileById, бо він має включати 'posts'
                var user = _userService.GetUserProfileById(userId);

                if (user != null)
                {
                    // Заповнюємо TextBlock'и даними
                    txtNameSidebar.Text = user.name;
                    txtNameMain.Text = user.name;
                    txtRegion.Text = user.region ?? "—"; // (?? "—" означає "якщо null, то поставити "—")
                    txtDistrict.Text = user.district ?? "—";
                    txtCity.Text = user.city ?? "—";
                    txtPhone.Text = user.phone ?? "—";
                    txtEmail.Text = user.email;
                    txtAbout.Text = user.about ?? "Інформація про себе ще не заповнена.";

                    // TODO: Тут вам ще треба буде завантажити список книг (posts)
                    // var books = await _bookService.GetBooksByUserIdAsync(userId);
                    // BooksList.ItemsSource = books; // (якщо у вас є BooksList)
                }
                else
                {
                    MessageBox.Show($"Тестовий користувач з ID {userId} не знайдений.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Це покаже помилку, якщо, наприклад, немає зв'язку з БД
                MessageBox.Show($"Сталася помилка при завантаженні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // --- ЗАГЛУШКИ ДЛЯ КНОПОК (Залишаються, як у вас) ---

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут відкриється вікно 'Додати книгу'");
            // ...
        }

        private void MyBooksButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут відкриється вікно 'Мої книги'");
            // ...
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут відкриється вікно 'Редагувати профіль'");
            // ...
        }
    }
}