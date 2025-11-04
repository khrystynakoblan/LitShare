using LitShare.BLL.Services;
using System.Threading.Tasks;
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ProfileViewWindow : Window
    {
        private readonly UserService _userService = new UserService();
        private readonly BookService _bookService = new BookService();
        private readonly int _userId;

        public ProfileViewWindow(int userBookId, int userId)
        {
            InitializeComponent();
            _userId = userId;
            _ = LoadUserProfileAsync(userBookId);
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
            var mainPage = new MainPage(_userId);
            mainPage.Show();
            this.Close();
        }

        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_userId);
            profileWindow.ShowDialog();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
