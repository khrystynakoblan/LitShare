// <copyright file="EditProfleWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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
    /// Interaction logic for EditProfileWindow.xaml. This window allows the user to modify their profile details.
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        // SA1214 - Readonly fields first (SA1309 - private fields without underscore)
        private readonly UserService userService = new UserService();
        private readonly int userId;

        // SA1214 - Non-readonly fields
        private Users currentUser;
        private Users originalUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditProfileWindow"/> class.
        /// </summary>
        /// <param name="userId">The ID of the currently logged-in user.</param>
        public EditProfileWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.LoadUserData(this.userId);
        }

        /// <summary>
        /// Loads the current user's data from the service into the window controls and saves the original state.
        /// </summary>
        /// <param name="userId">The ID of the user whose data should be loaded.</param>
        private void LoadUserData(int userId)
        {
            this.currentUser = this.userService.GetUserById(userId);
            if (this.currentUser == null)
            {
                return;
            }

            this.txtFirstName.Text = this.currentUser.Name;
            this.txtRegion.Text = this.currentUser.Region;
            this.txtDistrict.Text = this.currentUser.District;
            this.txtCity.Text = this.currentUser.City;
            this.txtPhone.Text = this.currentUser.Phone;
            this.txtAbout.Text = this.currentUser.About ?? string.Empty; // SA1122: Use string.Empty

            if (!string.IsNullOrEmpty(this.currentUser.PhotoUrl))
            {
                this.userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(this.currentUser.PhotoUrl)));
            }

            // Copy current user data to originalUser for restoration on Cancel
            this.originalUser = new Users
            {
                Id = this.currentUser.Id,
                Name = this.currentUser.Name,
                Email = this.currentUser.Email,
                Phone = this.currentUser.Phone,
                Password = this.currentUser.Password,
                Region = this.currentUser.Region,
                District = this.currentUser.District,
                City = this.currentUser.City,
                About = this.currentUser.About,
                PhotoUrl = this.currentUser.PhotoUrl,
            };
        }

        /// <summary>
        /// Validates the content of a specific input field and updates the corresponding error label.
        /// </summary>
        /// <param name="sender">The control (TextBox) to validate.</param>
        /// <param name="e">The event data (can be null).</param>
        private void ValidateField(object sender, RoutedEventArgs? e)
        {
            if (sender == this.txtRegion)
            {
                this.errRegion.Text = string.IsNullOrWhiteSpace(this.txtRegion.Text) ? "Введіть область" : string.Empty;
            }

            if (sender == this.txtDistrict)
            {
                this.errDistrict.Text = string.IsNullOrWhiteSpace(this.txtDistrict.Text) ? "Введіть район" : string.Empty;
            }

            if (sender == this.txtCity)
            {
                this.errCity.Text = string.IsNullOrWhiteSpace(this.txtCity.Text) ? "Введіть місто" : string.Empty;
            }

            if (sender == this.txtPhone)
            {
                this.errPhone.Text = Regex.IsMatch(this.txtPhone.Text, @"^\+?\d{10,13}$") ? string.Empty : "Некоректний номер телефону";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate all required fields
            this.ValidateField(this.txtRegion, null);
            this.ValidateField(this.txtDistrict, null);
            this.ValidateField(this.txtCity, null);
            this.ValidateField(this.txtPhone, null);

            // Check if any errors exist
            if (!string.IsNullOrEmpty(this.errRegion.Text) ||
                !string.IsNullOrEmpty(this.errDistrict.Text) ||
                !string.IsNullOrEmpty(this.errCity.Text) ||
                !string.IsNullOrEmpty(this.errPhone.Text))
            {
                return;
            }

            // Update user object
            this.currentUser.Region = this.txtRegion.Text;
            this.currentUser.District = this.txtDistrict.Text;
            this.currentUser.City = this.txtCity.Text;
            this.currentUser.Phone = this.txtPhone.Text;
            this.currentUser.About = this.txtAbout.Text;

            try
            {
                this.userService.UpdateUser(this.currentUser);
                MessageBox.Show("Профіль успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // SA1000 - keyword 'new' should be followed by a space
                MessageBox.Show($"Помилка при збереженні профілю: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var profilePage = new ProfileWindow(this.userId);
            profilePage.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Restore original values (optional, since we are closing and reopening ProfileWindow anyway, but good practice)
            this.txtFirstName.Text = this.originalUser.Name;
            this.txtRegion.Text = this.originalUser.Region;
            this.txtDistrict.Text = this.originalUser.District;
            this.txtCity.Text = this.originalUser.City;
            this.txtPhone.Text = this.originalUser.Phone;
            this.txtAbout.Text = this.originalUser.About ?? string.Empty;

            if (!string.IsNullOrEmpty(this.originalUser.PhotoUrl))
            {
                this.userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(this.originalUser.PhotoUrl)));
            }

            var profilePage = new ProfileWindow(this.userId);
            profilePage.Show();
            this.Close();
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            string randomUrl = $"https://randomuser.me/api/portraits/lego/{new Random().Next(0, 9)}.jpg";
            this.userPhotoEllipse.Fill = new ImageBrush(new BitmapImage(new Uri(randomUrl)));
            this.currentUser.PhotoUrl = randomUrl;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentUser == null)
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
                    this.userService.DeleteUser(this.currentUser.Id);

                    // Assuming AuthWindow is the initial login/registration window
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Simply closing this window and showing the profile window is enough,
            // no need for async/await here unless the original code required a delay for UI effect.
            // If the original intent was a smooth transition, we keep async/await.
            var profilePage = new ProfileWindow(this.userId);
            profilePage.Show();
            this.Close();
        }
    }
}