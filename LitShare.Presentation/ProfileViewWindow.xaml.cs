using LitShare.BLL.Services;
using System.Threading.Tasks;
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ProfileViewWindow : Window
    {
        private readonly UserService _userService = new UserService();
        private readonly BookService _bookService = new BookService();

        public ProfileViewWindow()
        {
            InitializeComponent();

            int testUserId = 1; // 🔹 тестовий ID користувача
            _ = LoadUserProfileAsync(testUserId);
        }

        private async Task LoadUserProfileAsync(int userId)
        {
            var user = _userService.GetUserProfileById(userId);

            if (user != null)
            {
                txtName.Text = user.name;
                txtRegion.Text = user.region;
                txtDistrict.Text = user.district;
                txtCity.Text = user.city;
                txtPhone.Text = user.phone ?? "—";
                txtAbout.Text = user.about ?? "Користувач ще не заповнив інформацію про себе.";

                // 🔹 асинхронне завантаження книг
                var books = await _bookService.GetBooksByUserIdAsync(userId);
                BooksList.ItemsSource = books;
            }
            else
            {
                MessageBox.Show("Користувача не знайдено!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // спочатку сховати
            var mainPage = new MainPage();
            mainPage.Show();
            this.Close(); // а тоді повністю закрити
        }


        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ви вже переглядаєте свій профіль.");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Назад до попереднього вікна (ще не реалізовано).");
        }
    }
}
