using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LitShare.BLL.Services;
using LitShare.DAL.Models;

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

            LoadUserData(userId); 
            _userId = userId;
        }

        private void LoadUserData(int userId)
        {
            _currentUser = _userService.GetUserById(userId);
            if (_currentUser == null)
            {
                MessageBox.Show("Користувача не знайдено.", "Помилка");
                return;
            }

            txtFirstName.Text = _currentUser.name;
            txtRegion.Text = _currentUser.region;
            txtDistrict.Text = _currentUser.district;
            txtCity.Text = _currentUser.city;
            txtPhone.Text = _currentUser.phone;
            txtAbout.Text = _currentUser.about ?? "";

            if (!string.IsNullOrEmpty(_currentUser.photo_url))
            {
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_currentUser.photo_url)));
            }
            else
            {
                string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
                _currentUser.photo_url = randomUrl;
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));
            }

            _originalUser = new Users
            {
                id = _currentUser.id,
                name = _currentUser.name,
                email = _currentUser.email,
                phone = _currentUser.phone,
                password = _currentUser.password,
                region = _currentUser.region,
                district = _currentUser.district,
                city = _currentUser.city,
                about = _currentUser.about,
                photo_url = _currentUser.photo_url
            };
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
            userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));

            if (_currentUser != null)
                _currentUser.photo_url = randomUrl;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Дані користувача не завантажено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentUser.name = txtFirstName.Text;
            _currentUser.region = txtRegion.Text;
            _currentUser.district = txtDistrict.Text;
            _currentUser.city = txtCity.Text;
            _currentUser.phone = txtPhone.Text;
            _currentUser.about = txtAbout.Text;

            if (string.IsNullOrEmpty(_currentUser.photo_url))
                _currentUser.photo_url = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";

            try
            {
                _userService.UpdateUser(_currentUser);
                MessageBox.Show("Зміни успішно збережено!", "LitShare", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.DialogResult = true; 
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_originalUser == null)
                return;

            txtFirstName.Text = _originalUser.name;
            txtRegion.Text = _originalUser.region;
            txtDistrict.Text = _originalUser.district;
            txtCity.Text = _originalUser.city;
            txtPhone.Text = _originalUser.phone;
            txtAbout.Text = _originalUser.about ?? "";

            if (!string.IsNullOrEmpty(_originalUser.photo_url))
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_originalUser.photo_url)));

            MessageBox.Show("Зміни скасовано. Дані відновлено.", "LitShare", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Користувач не завантажений.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Ви впевнені, що хочете видалити профіль?\nЦю дію не можна скасувати.",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _userService.DeleteUser(_currentUser.id);
                    MessageBox.Show("Профіль успішно видалено!", "LitShare", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            var authPage = new AuthWindow();
            authPage.Show();
            this.Close();
        }

        private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                await Task.Delay(150);
                var mainPage = new MainPage(_userId);
                mainPage.Show();
                this.Close();
            }
            catch
            {
                this.Show();
                MessageBox.Show("Головна сторінка ще не реалізована.", "LitShare");
            }
        }

    }
}
