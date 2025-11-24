using LitShare.BLL.Services;
using LitShare.DAL.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class EditProfileWindow : Window
    {
        private readonly UserService _userService = new UserService();
        private Users _currentUser;
        private Users _originalUser;
        private readonly int _userId;

        public EditProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUserData(userId);
        }

        private void LoadUserData(int userId)
        {
            _currentUser = _userService.GetUserById(userId);
            if (_currentUser == null)
                return;

            txtFirstName.Text = _currentUser.Name;
            txtRegion.Text = _currentUser.Region;
            txtDistrict.Text = _currentUser.District;
            txtCity.Text = _currentUser.City;
            txtPhone.Text = _currentUser.Phone;
            txtAbout.Text = _currentUser.About ?? "";

            if (!string.IsNullOrEmpty(_currentUser.PhotoUrl))
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_currentUser.PhotoUrl)));

            _originalUser = new Users
            {
                Id = _currentUser.Id,
                Name = _currentUser.Name,
                Email = _currentUser.Email,
                Phone = _currentUser.Phone,
                Password = _currentUser.Password,
                Region = _currentUser.Region,
                District = _currentUser.District,
                City = _currentUser.City,
                About = _currentUser.About,
                PhotoUrl = _currentUser.PhotoUrl
            };
        }

        private void ValidateField(object sender, RoutedEventArgs e)
        {
            if (sender == txtRegion)
                errRegion.Text = string.IsNullOrWhiteSpace(txtRegion.Text) ? "Введіть область" : "";

            if (sender == txtDistrict)
                errDistrict.Text = string.IsNullOrWhiteSpace(txtDistrict.Text) ? "Введіть район" : "";

            if (sender == txtCity)
                errCity.Text = string.IsNullOrWhiteSpace(txtCity.Text) ? "Введіть місто" : "";

            if (sender == txtPhone)
                errPhone.Text = Regex.IsMatch(txtPhone.Text, @"^\+?\d{10,13}$") ? "" : "Некоректний номер телефону";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateField(txtRegion, null);
            ValidateField(txtDistrict, null);
            ValidateField(txtCity, null);
            ValidateField(txtPhone, null);

            if (!string.IsNullOrEmpty(errRegion.Text) ||
                !string.IsNullOrEmpty(errDistrict.Text) ||
                !string.IsNullOrEmpty(errCity.Text) ||
                !string.IsNullOrEmpty(errPhone.Text))
                return;

            _currentUser.Region = txtRegion.Text;
            _currentUser.District = txtDistrict.Text;
            _currentUser.City = txtCity.Text;
            _currentUser.Phone = txtPhone.Text;
            _currentUser.About = txtAbout.Text;

            try
            {
                _userService.UpdateUser(_currentUser);
            }
            catch
            {
            }

            var profilePage = new ProfileWindow(_userId);
            profilePage.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            txtRegion.Text = _originalUser.Region;
            txtDistrict.Text = _originalUser.District;
            txtCity.Text = _originalUser.City;
            txtPhone.Text = _originalUser.Phone;
            txtAbout.Text = _originalUser.About ?? "";

            if (!string.IsNullOrEmpty(_originalUser.PhotoUrl))
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_originalUser.PhotoUrl)));

            var profilePage = new ProfileWindow(_userId);
            profilePage.Show();
            this.Close();
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
            userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));
            _currentUser.PhotoUrl = randomUrl;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var result = MessageBox.Show(
                "Ви дійсно хочете видалити профіль?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _userService.DeleteUser(_currentUser.Id);

                    var authWindow = new AuthWindow();
                    authWindow.Show();

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталася помилка при видаленні профілю:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                await Task.Delay(150);
                var profilePage = new ProfileWindow(_userId);
                profilePage.Show();
                this.Close();
            }
            catch
            {
                this.Show();
            }
        }
    }
}
