using LitShare.BLL.Services; // 1. ДОДАНО: Підключаємо BLL
using LitShare.DAL.Models;
using System.Threading.Tasks;  // 2. ДОДАНО: Потрібно для асинхронності
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class ProfileWindow : Window
    {
        // 3. ДОДАНО: Створюємо екземпляр сервісу
        private readonly UserService _userService = new UserService();
        private readonly int _userId;


        public ProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _ = LoadUserProfileAsync(_userId);
        }

        // 5. ДОДАНО: Новий метод для завантаження даних
        private async Task LoadUserProfileAsync(int userId)
        {
            try
            {
                var user = _userService.GetUserProfileById(userId);

                if (user != null)
                {
                    txtNameSidebar.Text = user.name;
                    txtNameMain.Text = user.name;
                    txtRegion.Text = user.region ?? "—";
                    txtDistrict.Text = user.district ?? "—";
                    txtCity.Text = user.city ?? "—";
                    txtPhone.Text = user.phone ?? "—";
                    txtEmail.Text = user.email;
                    txtAbout.Text = user.about ?? "Інформація про себе ще не заповнена.";

                    if (!string.IsNullOrEmpty(user.photo_url))
                    {
                        userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(user.photo_url)));
                    }
                    else
                    {
                        string defaultUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
                        userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(defaultUrl)));
                    }
                }
                else
                {
                    MessageBox.Show($"Користувача з ID {userId} не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
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
            var newAdWindow = new NewAdWindow(_userId);
            bool? result = newAdWindow.ShowDialog();

            if (result == true)
            {
                var myBooksWindow = new MyBook(_userId);
                myBooksWindow.Show();
                this.Close();
            }
        }


        private void MyBooksButton_Click(object sender, RoutedEventArgs e)
        {
            var myBooksWindow = new MyBook(_userId);
            myBooksWindow.Show();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var editProfileWindow = new EditProfileWindow(_userId);
            bool? result = editProfileWindow.ShowDialog(); 

            if (result == true)
            {
                _ = LoadUserProfileAsync(_userId);
            }
        }
    }
}