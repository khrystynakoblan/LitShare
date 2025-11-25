// <copyright file="ProfileViewWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for ProfileViewWindow.xaml.
    /// This window displays the public profile and book listings of another user.
    /// </summary>
    public partial class ProfileViewWindow : Window
    {
        /// <summary>
        /// The service for user-related operations.
        /// </summary>
        private readonly UserService userService = new UserService();

        /// <summary>
        /// The service for book/ad-related operations.
        /// </summary>
        private readonly BookService bookService = new BookService();

        /// <summary>
        /// The ID of the currently logged-in user.
        /// </summary>
        private readonly int userId;

        /// <summary>
        /// The ID of the user whose profile is being viewed.
        /// </summary>
        private readonly int userBookId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileViewWindow"/> class.
        /// </summary>
        /// <param name="userBookId">The ID of the user whose profile is to be viewed.</param>
        /// <param name="userId">The ID of the currently logged-in user.</param>
        public ProfileViewWindow(int userBookId, int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            this.userBookId = userBookId;
            _ = this.LoadUserProfileAsync(userBookId);
        }

        /// <summary>
        /// Asynchronously loads the target user's profile information and their book listings.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile is to be loaded.</param>
        /// <returns>A task representing the asynchronous loading operation.</returns>
        private async Task LoadUserProfileAsync(int userId)
        {
            try
            {
                // Note: GetUserProfileById is assumed to return a complete UserProfileDto or null.
                var user = await Task.Run(() => this.userService.GetUserProfileById(userId));

                if (user != null)
                {
                    // Populate user information fields
                    this.txtName.Text = user.Name;
                    this.txtRegion.Text = user.Region;
                    this.txtDistrict.Text = user.District;
                    this.txtCity.Text = user.City;
                    this.txtPhone.Text = user.Phone ?? "—";
                    this.txtAbout.Text = user.About ?? "Користувач ще не заповнив інформацію про себе."; // Default text in Ukrainian

                    // Load user's books
                    var books = await this.bookService.GetBooksByUserIdAsync(userId);
                    this.BooksList.ItemsSource = books;
                }
                else
                {
                    MessageBox.Show("Користувача не знайдено!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the profile: {ex.Message}", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Handles the click event for the Home button.
        /// Navigates the current user back to the main application page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(this.userId);
            mainPage.Show();
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the My Profile button.
        /// Opens the currently logged-in user's profile edit window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(this.userId);
            profileWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the click event for the Back button.
        /// Closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the "View Ad" context menu item on a book.
        /// Opens the <see cref="ViewAdWindow"/> for the selected book.
        /// </summary>
        /// <param name="sender">The source of the event (the MenuItem).</param>
        /// <param name="e">The routing event data.</param>
        private void ContextMenu_View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var viewWindow = new ViewAdWindow(book.Id, this.userId);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the click event for the "Report Ad" context menu item on a book.
        /// Opens the <see cref="ReportAdWindow"/> for the selected book.
        /// </summary>
        /// <param name="sender">The source of the event (the MenuItem).</param>
        /// <param name="e">The routing event data.</param>
        private void ContextMenu_Report_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var reportWindow = new ReportAdWindow(book.Id, this.userId);
                reportWindow.Owner = this;
                reportWindow.ShowDialog();
            }
        }
    }
}