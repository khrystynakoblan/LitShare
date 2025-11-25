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
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for ProfileWindow.xaml.
    /// </summary>
    public partial class ProfileWindow : Window
    {
        // SA1214 - Readonly fields first (SA1309 - private fields without underscore)
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
            _ = this.LoadUserProfileAsync(this.userId);
        }

        /// <summary>
        /// Asynchronously loads the current user's profile information and updates the UI elements.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile should be loaded.</param>
        /// <returns>A task that represents the asynchronous loading operation.</returns>
        private async Task LoadUserProfileAsync(int userId)
        {
            try
            {
                // ПОМИЛКА: CS1061. Прибрано 'await', оскільки GetUserProfileById, ймовірно, синхронний.
                // Якщо GetUserProfileById є асинхронним, його сигнатура має бути public Task<UserDto> GetUserProfileByIdAsync(int userId).
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
                    }
                    else
                    {
                        // SA1000 - keyword 'new' should be followed by a space
                        string defaultUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
                        this.userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(defaultUrl)));
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
            var newAdWindow = new NewAdWindow(this.userId);
            bool? result = newAdWindow.ShowDialog();

            if (result == true)
            {
                var myBooksWindow = new MyBook(this.userId);
                myBooksWindow.Show();
                this.Close();
            }
        }

        private void MyBooksButton_Click(object sender, RoutedEventArgs e)
        {
            var myBooksWindow = new MyBook(this.userId);
            myBooksWindow.Show();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(this.userId);
            mainPage.Show();
            this.Close();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var editProfileWindow = new EditProfileWindow(this.userId);
            bool? result = editProfileWindow.ShowDialog();

            if (result == true)
            {
                _ = this.LoadUserProfileAsync(this.userId);
            }
        }
    }
}