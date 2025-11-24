using LitShare.BLL.DTOs;
using LitShare.BLL.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
                txtName.Text = user.Name;
                txtRegion.Text = user.Region;
                txtDistrict.Text = user.District;
                txtCity.Text = user.City;
                txtPhone.Text = user.Phone ?? "—";
                txtAbout.Text = user.About ?? "Користувач ще не заповнив інформацію про себе.";

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

        private void ContextMenu_View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var viewWindow = new ViewAdWindow(book.Id, _userId);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
            }
        }

        private void ContextMenu_Report_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var reportWindow = new ReportAdWindow(book.Id, _userId);
                reportWindow.Owner = this;
                reportWindow.ShowDialog();
            }
        }

    }
}
