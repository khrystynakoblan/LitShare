using System;
using System.Linq;
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
        private Users _currentUser;   // поточний користувач
        private Users _originalUser;  // копія для "Скасувати"

        public EditProfileWindow()
        {
            InitializeComponent();
            LoadUserData(1); //  тестовий ID користувача
        }

        //  Завантаження даних користувача
        private void LoadUserData(int userId)
        {
            _currentUser = _userService.GetUserById(userId);
            if (_currentUser == null)
            {
                MessageBox.Show("Користувача не знайдено.", "Помилка");
                return;
            }

            // Розбиваємо ім’я на частини (якщо записано через пробіл)
            txtFirstName.Text = _currentUser.name.Split(' ').FirstOrDefault() ?? _currentUser.name;
            txtLastName.Text = _currentUser.name.Split(' ').Skip(1).FirstOrDefault() ?? "";

            txtRegion.Text = _currentUser.region;
            txtDistrict.Text = _currentUser.district;
            txtCity.Text = _currentUser.city;
            txtPhone.Text = _currentUser.phone;
            txtAbout.Text = _currentUser.about ?? "";

            // Встановлення аватарки
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

            // Створюємо копію користувача для кнопки "Скасувати"
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

        // Зміна фото
        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
            userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));

            if (_currentUser != null)
                _currentUser.photo_url = randomUrl; //  зберігаємо новий URL фото
        }

        //  Зберегти зміни в базу
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Дані користувача не завантажено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Оновлюємо дані користувача
            _currentUser.region = txtRegion.Text;
            _currentUser.district = txtDistrict.Text;
            _currentUser.city = txtCity.Text;
            _currentUser.phone = txtPhone.Text;
            _currentUser.about = txtAbout.Text;

            if (string.IsNullOrEmpty(_currentUser.photo_url))
                _currentUser.photo_url = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";

            try
            {
                _userService.UpdateUser(_currentUser); // запис у БД
                MessageBox.Show(" Зміни успішно збережено!", "LitShare", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //  Скасувати зміни
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_originalUser == null)
                return;

            //  Відновлюємо початкові дані
            txtFirstName.Text = _originalUser.name.Split(' ').FirstOrDefault() ?? _originalUser.name;
            txtLastName.Text = _originalUser.name.Split(' ').Skip(1).FirstOrDefault() ?? "";
            txtRegion.Text = _originalUser.region;
            txtDistrict.Text = _originalUser.district;
            txtCity.Text = _originalUser.city;
            txtPhone.Text = _originalUser.phone;
            txtAbout.Text = _originalUser.about ?? "";

            if (!string.IsNullOrEmpty(_originalUser.photo_url))
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(_originalUser.photo_url)));

            MessageBox.Show(" Зміни скасовано. Дані відновлено.", "LitShare", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 🔹Видалити профіль (тест)
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
            else
            {
                MessageBox.Show("Видалення скасовано.", "LitShare", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        
         private async void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                await Task.Delay(150); // невелика затримка для плавності
                var mainPage = new MainPage();
                mainPage.Show();
                this.Close();
            }
            catch
            {
                this.Show();
                MessageBox.Show("Головна сторінка ще не реалізована.", "LitShare");
            }
        }


        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Повернення до профілю (тест).", "LitShare");
        }
    }
}