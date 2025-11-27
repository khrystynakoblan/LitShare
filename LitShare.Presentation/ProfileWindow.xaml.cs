// <copyright file="ProfileWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.Logging;
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for ProfileWindow.xaml.
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly UserService userService = new UserService();
        private readonly int userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileWindow"/> class.
        /// </summary>
        /// <param name="userId">The ID of the currently logged-in user.</param>
        public ProfileWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;

            AppLogger.Info($"Відкрито ProfileWindow для користувача ID = {userId}");

            _ = this.LoadUserProfileAsync(this.userId);
        }

        /// <summary>
        /// Asynchronously loads the current user's profile information and updates the UI elements.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile should be loaded.</param>
        /// <returns>A task that represents the asynchronous loading operation.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task LoadUserProfileAsync(int userId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                AppLogger.Info($"Початок завантаження профілю користувача ID = {userId}");

                var user = this.userService.GetUserProfileById(userId);

                if (user != null)
                {
                    this.txtNameSidebar.Text = user.Name;
                    this.txtNameMain.Text = user.Name;
                    this.txtRegion.Text = user.Region ?? "—";
                    this.txtDistrict.Text = user.District ?? "—";
                    this.txtCity.Text = user.City ?? "—";
                    this.txtPhone.Text = user.Phone ?? "—";
                    this.txtEmail.Text = user.Email;
                    this.txtAbout.Text = user.About ?? "Інформація про себе ще не заповнена.";

                    if (!string.IsNullOrEmpty(user.PhotoUrl))
                    {
                        this.userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(user.PhotoUrl)));

                        AppLogger.Info($"Завантажено фото профілю користувача ID = {userId}");
                    }
                    else
                    {
                        string defaultUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
                        this.userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(defaultUrl)));

                        AppLogger.Warn($"У користувача ID = {userId} відсутнє фото. Встановлено випадкове.");
                    }

                    AppLogger.Info($"Профіль користувача ID = {userId} успішно завантажено");
                }
                else
                {
                    AppLogger.Warn($"Користувача з ID = {userId} не знайдено");
                    MessageBox.Show($"Користувача з ID {userId} не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error($"ПОМИЛКА при завантаженні профілю користувача ID = {userId}: {ex}");
                MessageBox.Show($"Сталася помилка при завантаженні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Натиснуто Назад у ProfileWindow (користувач ID = {this.userId})");
            this.Close();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Перехід до створення оголошення. Користувач ID = {this.userId}");

            var newAdWindow = new NewAdWindow(this.userId);
            bool? result = newAdWindow.ShowDialog();

            if (result == true)
            {
                AppLogger.Info($"Оголошення успішно додано користувачем ID = {this.userId}");

                var myBooksWindow = new MyBook(this.userId);
                myBooksWindow.Show();
                this.Close();
            }
            else
            {
                AppLogger.Warn($"Створення оголошення скасовано користувачем ID = {this.userId}");
            }
        }

        private void MyBooksButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Відкрито список моїх книг користувачем ID = {this.userId}");

            var myBooksWindow = new MyBook(this.userId);
            myBooksWindow.Show();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Перехід на головну з ProfileWindow. Користувач ID = {this.userId}");

            var mainPage = new MainPage(this.userId);
            mainPage.Show();
            this.Close();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Відкрито редагування профілю користувача ID = {this.userId}");

            var editProfileWindow = new EditProfileWindow(this.userId);
            bool? result = editProfileWindow.ShowDialog();

            if (result == true)
            {
                AppLogger.Info($"Профіль користувача ID = {this.userId} змінено. Перезавантаження даних.");

                _ = this.LoadUserProfileAsync(this.userId);
            }
            else
            {
                AppLogger.Warn($"Редагування профілю скасовано користувачем ID = {this.userId}");
            }
        }
    }
}