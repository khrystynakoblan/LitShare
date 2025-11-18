// -----------------------------------------------------------------------
// <copyright file="EditProfileWindow.xaml.cs" company="LitShare">
// Copyright (c) 2025 LitShare. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.Presentation
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;

    /// <summary>
    /// Логіка взаємодії для вікна редагування профілю користувача.
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        private readonly UserService _userService = new UserService();
        private readonly int _userId;
        private Users? _currentUser;
        private Users? _originalUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditProfileWindow"/> class.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача, профіль якого редагується.</param>
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
            {
                MessageBox.Show("Не вдалося завантажити дані користувача.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            // Заповнення полів форми
            txtFirstName.Text = _currentUser.Name;
            txtRegion.Text = _currentUser.Region;
            txtDistrict.Text = _currentUser.District;
            txtCity.Text = _currentUser.City;
            txtPhone.Text = _currentUser.Phone;
            txtAbout.Text = _currentUser.About ?? string.Empty;

            if (!string.IsNullOrEmpty(_currentUser.PhotoUrl))
            {
                try
                {
                    userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_currentUser.PhotoUrl)));
                }
                catch
                {
                    // Ігноруємо помилки завантаження зображення, залишаємо дефолтне
                }
            }

            // Створення копії для можливості скасування змін
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

        private void ValidateField(object sender, RoutedEventArgs? e)
        {
            if (sender == txtRegion)
            {
                errRegion.Text = string.IsNullOrWhiteSpace(txtRegion.Text) ? "Введіть область" : string.Empty;
            }

            if (sender == txtDistrict)
            {
                errDistrict.Text = string.IsNullOrWhiteSpace(txtDistrict.Text) ? "Введіть район" : string.Empty;
            }

            if (sender == txtCity)
            {
                errCity.Text = string.IsNullOrWhiteSpace(txtCity.Text) ? "Введіть місто" : string.Empty;
            }

            if (sender == txtPhone)
            {
                errPhone.Text = Regex.IsMatch(txtPhone.Text, @"^\+?\d{10,13}$") ? string.Empty : "Некоректний номер телефону";
            }
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
            {
                MessageBox.Show("Будь ласка, виправте помилки у формі.", "Валідація", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentUser == null)
            {
                return;
            }

            _currentUser.Region = txtRegion.Text;
            _currentUser.District = txtDistrict.Text;
            _currentUser.City = txtCity.Text;
            _currentUser.Phone = txtPhone.Text;
            _currentUser.About = txtAbout.Text;

            try
            {
                _userService.UpdateUser(_currentUser);

                var profilePage = new ProfileWindow(_userId);
                profilePage.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні змін: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_originalUser == null)
            {
                Close();
                return;
            }

            txtRegion.Text = _originalUser.Region;
            txtDistrict.Text = _originalUser.District;
            txtCity.Text = _originalUser.City;
            txtPhone.Text = _originalUser.Phone;
            txtAbout.Text = _originalUser.About ?? string.Empty;

            if (!string.IsNullOrEmpty(_originalUser.PhotoUrl))
            {
                try
                {
                    userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_originalUser.PhotoUrl)));
                }
                catch
                {
                }
            }

            var profilePage = new ProfileWindow(_userId);
            profilePage.Show();
            Close();
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                return;
            }

            string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";

            try
            {
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));
                _currentUser.PhotoUrl = randomUrl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося змінити фото: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                return;
            }

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

                    Close();
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
                Hide();
                await Task.Delay(150); 

                var profilePage = new ProfileWindow(_userId);
                profilePage.Show();
                Close();
            }
            catch (Exception ex)
            {
                Show(); 
                MessageBox.Show($"Помилка переходу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}