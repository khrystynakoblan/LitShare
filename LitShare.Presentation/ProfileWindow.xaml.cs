// -----------------------------------------------------------------------
// <copyright file="ProfileWindow.xaml.cs" company="LitShare">
// Copyright (c) 2025 LitShare. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.Presentation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.Services;

    /// <summary>
    /// Логіка взаємодії для вікна профілю користувача.
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly UserService _userService = new UserService();
        private readonly int _userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileWindow"/> class.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        public ProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadUserProfile(_userId); // Виклик синхронного методу
        }

        // ВИПРАВЛЕННЯ CS1998: Прибрано 'async Task', оскільки метод не має асинхронних викликів
        private void LoadUserProfile(int userId)
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
                        try
                        {
                            userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(user.PhotoUrl)));
                        }
                        catch
                        {
                            // Якщо посилання бите, ставимо дефолтне
                            SetDefaultPhoto();
                        }
                    }
                    else
                    {
                        SetDefaultPhoto();
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

        private void SetDefaultPhoto()
        {
            string defaultUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
            try
            {
                userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(defaultUrl)));
            }
            catch
            {
                // Ігноруємо, якщо навіть дефолтне фото не вантажиться
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var newAdWindow = new NewAdWindow(_userId);
            bool? result = newAdWindow.ShowDialog();

            if (result == true)
            {
                var myBooksWindow = new MyBook(_userId);
                myBooksWindow.Show();
                Close();
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
            Close();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var editProfileWindow = new EditProfileWindow(_userId);
            bool? result = editProfileWindow.ShowDialog();

            if (result == true)
            {
                LoadUserProfile(_userId);
            }
        }
    }
}