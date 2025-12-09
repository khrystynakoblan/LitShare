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
    using LitShare.BLL.Logging;
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
        private readonly UserService userService = new ();

        /// <summary>
        /// The service for book/ad-related operations.
        /// </summary>
        private readonly BookService bookService = new ();

        /// <summary>
        /// The ID of the currently logged-in user.
        /// </summary>
        private readonly int userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileViewWindow"/> class.
        /// </summary>
        /// <param name="userBookId">The ID of the user whose profile is to be viewed.</param>
        /// <param name="userId">The ID of the currently logged-in user.</param>
        public ProfileViewWindow(int userBookId, int userId)
        {
            this.InitializeComponent();
            this.userId = userId;

            AppLogger.Info($"Відкрито ProfileViewWindow. Перегляд профілю користувача ID = {userBookId}, поточний користувач ID = {userId}");

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
                AppLogger.Info($"Початок завантаження профілю користувача ID = {userId}");

                var user = await Task.Run(() => this.userService.GetUserProfileById(userId));

                if (user != null)
                {
                    this.txtName.Text = user.Name;
                    this.txtRegion.Text = user.Region;
                    this.txtDistrict.Text = user.District;
                    this.txtCity.Text = user.City;
                    this.txtPhone.Text = user.Phone ?? "—";
                    this.txtAbout.Text = user.About ?? "Користувач ще не заповнив інформацію про себе."; // Default text in Ukrainian

                    var books = await this.bookService.GetBooksByUserIdAsync(userId);
                    this.BooksList.ItemsSource = books;

                    AppLogger.Info($"Профіль користувача ID = {userId} успішно завантажено. Книг: {books.Count}");
                }
                else
                {
                    AppLogger.Warn($"Користувача з ID = {userId} не знайдено");

                    MessageBox.Show("Користувача не знайдено!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error($"ПОМИЛКА при завантаженні профілю користувача ID = {userId}: {ex}");

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
            AppLogger.Info($"Перехід на головну з ProfileViewWindow. Користувач ID = {this.userId}");
            NavigationManager.GoToMainPage(this.userId);
        }

        /// <summary>
        /// Handles the click event for the My Profile button.
        /// Opens the currently logged-in user's profile edit window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Перехід до власного профілю з ProfileViewWindow. Користувач ID = {this.userId}");

            var profileWindow = new ProfileWindow(this.userId);
            NavigationManager.NavigateTo(profileWindow, this);
        }

        /// <summary>
        /// Handles the click event for the Back button.
        /// Closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info("Закриття ProfileViewWindow кнопкою Назад");
            NavigationManager.GoBack();
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
                AppLogger.Info($"Перегляд оголошення ID = {book.Id} користувачем ID = {this.userId}");
                var viewWindow = new ViewAdWindow(book.Id, this.userId);
                NavigationManager.NavigateTo(viewWindow, this);
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
                AppLogger.Warn($"СКАРГА на оголошення ID = {book.Id} від користувача ID = {this.userId}");

                var reportWindow = new ReportAdWindow(book.Id, this.userId);
                NavigationManager.ShowDialog(reportWindow, this);
            }
            else
            {
                AppLogger.Warn("Спроба скарги на оголошення без вибраної книги");
            }
        }
    }
}