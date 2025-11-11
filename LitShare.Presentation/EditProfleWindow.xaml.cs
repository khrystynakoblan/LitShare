using System;
using System.Text.RegularExpressions;
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
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_currentUser.photo_url)));

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

        // Немодальна валідація полів
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

        // 🔹 Зберегти зміни
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
                return; // є помилки — не зберігаємо

            _currentUser.region = txtRegion.Text;
            _currentUser.district = txtDistrict.Text;
            _currentUser.city = txtCity.Text;
            _currentUser.phone = txtPhone.Text;
            _currentUser.about = txtAbout.Text;

            try
            {
                _userService.UpdateUser(_currentUser);
            }
            catch
            {
                // помилка можна залогувати
            }

            // Повернення на профіль користувача
            var profilePage = new ProfileWindow(_userId);
            profilePage.Show();
            this.Close();
        }

        // 🔹 Скасувати зміни
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            txtRegion.Text = _originalUser.region;
            txtDistrict.Text = _originalUser.district;
            txtCity.Text = _originalUser.city;
            txtPhone.Text = _originalUser.phone;
            txtAbout.Text = _originalUser.about ?? "";

            if (!string.IsNullOrEmpty(_originalUser.photo_url))
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_originalUser.photo_url)));

            // Повернення на профіль користувача
            var profilePage = new ProfileWindow(_userId);
            profilePage.Show();
            this.Close();
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
            userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));
            _currentUser.photo_url = randomUrl;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            // --- Модальне підтвердження ---
            var result = MessageBox.Show(
                "Ви дійсно хочете видалити профіль? Ця дія незворотна.",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _userService.DeleteUser(_currentUser.id);

                    // Перехід на вікно реєстрації
                    var authWindow = new AuthWindow();
                    authWindow.Show();

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталася помилка при видаленні профілю:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            // Якщо користувач натиснув "No" — нічого не робимо
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
