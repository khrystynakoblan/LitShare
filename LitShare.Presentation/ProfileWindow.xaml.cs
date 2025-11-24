using LitShare.BLL.Services; 
using System.Threading.Tasks;  
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class ProfileWindow : Window
    {
        private readonly UserService _userService = new UserService();
        private readonly int _userId;


        public ProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _ = LoadUserProfileAsync(_userId);
        }

        private async Task LoadUserProfileAsync(int userId)
        {
            try
            {
                var user = _userService.GetUserProfileById(userId);

                if (user != null)
                {
                    txtNameSidebar.Text = user.Name;
                    txtNameMain.Text = user.Name;
                    txtRegion.Text = user.Region ?? "—";
                    txtDistrict.Text = user.District ?? "—";
                    txtCity.Text = user.City ?? "—";
                    txtPhone.Text = user.Phone ?? "—";
                    txtEmail.Text = user.Email;
                    txtAbout.Text = user.About ?? "Інформація про себе ще не заповнена.";

                    if (!string.IsNullOrEmpty(user.PhotoUrl))
                    {
                        userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(user.PhotoUrl)));
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(_userId);
            mainPage.Show();
            this.Close();
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