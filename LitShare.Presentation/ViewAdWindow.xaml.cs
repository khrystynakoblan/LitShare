// <copyright file="ViewAdWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System.Windows;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Logging;
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for ViewAdWindow.xaml, which displays detailed information about a book advertisement.
    /// </summary>
    public partial class ViewAdWindow : Window
    {
        /// <summary>
        /// Service used for working with book data.
        /// </summary>
        private readonly BookService bookService = new ();

        /// <summary>
        /// Identifier of the currently logged-in user.
        /// </summary>
        private readonly int userId;

        /// <summary>
        /// Book data displayed in the window.
        /// </summary>
        private BookDto? currentBook;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAdWindow"/> class.
        /// Used for design-time support or when called from another constructor.
        /// </summary>
        public ViewAdWindow()
        {
            this.InitializeComponent();
            this.SetPlaceholder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAdWindow"/> class
        /// with the specified book and user identifiers.
        /// </summary>
        /// <param name="bookId">Identifier of the book to display.</param>
        /// <param name="userId">Identifier of the currently logged-in user.</param>

        public ViewAdWindow(int bookId, int userId)
            : this()
        {
            _ = this.LoadBook(bookId);
            this.userId = userId;

            AppLogger.Info($"Відкрито ViewAdWindow: BookId={bookId}, UserId={userId}");
        }

        /// <summary>
        /// Sets placeholder test values for the preview fields.
        /// </summary>
        private void SetPlaceholder()
        {
            this.TitleText.Text = "Назва (тест)";
            this.AuthorText.Text = "Автор (тест)";
            this.DescriptionText.Text = "Опис (тест)";
            this.DealTypeText.Text = "Тип угоди (тест)";
            this.LocationText.Text = "Місто (тест)";
            this.PostImage.Source = null;
        }

        /// <summary>
        /// Asynchronously loads book data by its identifier and displays it in the window.
        /// </summary>
        /// <param name="bookId">Identifier of the book.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadBook(int bookId)
        {
            AppLogger.Info($"Завантаження даних книги: BookId={bookId}");
            try
            {
                this.currentBook = await this.bookService.GetBookById(bookId);
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Помилка завантаження книги з БД: BookId={bookId}", ex);

                MessageBox.Show(
                    $"Помилка при зверненні до бази: {ex.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
                return;
            }

            if (this.currentBook == null)
            {
                AppLogger.Warn($"Книгу не знайдено в БД: BookId={bookId}");

                MessageBox.Show(
                    "Книга не знайдена.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                this.Close();
                return;
            }

            AppLogger.Info($"Книгу успішно завантажено: BookId={bookId}, Title='{this.currentBook.Title}'");

            this.TitleText.Text = this.currentBook.Title ?? string.Empty;
            this.AuthorText.Text = this.currentBook.Author ?? string.Empty;
            this.DescriptionText.Text = this.currentBook.Description ?? string.Empty;
            this.DealTypeText.Text = this.currentBook.DealType ?? string.Empty;
            this.LocationText.Text = this.currentBook.Location ?? string.Empty;

            this.LoadBookImage(this.currentBook.ImagePath);
        }

        /// <summary>
        /// Loads the book image from the provided URL path.
        /// </summary>
        /// <param name="url">Path to the image (local or absolute).</param>
        private void LoadBookImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                this.PostImage.Source = null;
                return;
            }

            try
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                this.PostImage.Source = img;
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"Не вдалося завантажити зображення книги: URL={url}, Error={ex.Message}");

                this.PostImage.Source = null;
            }
        }

        /// <summary>
        /// Handles the click event of the "User Profile" button.
        /// Opens the profile of the user who created the advertisement.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Routed event data.</param>
        private void UserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentBook != null)
            {
                AppLogger.Info($"Перегляд профілю автора оголошення: BookId={this.currentBook.Id}, AuthorUserId={this.currentBook.UserId}, ViewerUserId={this.userId}");

                var profileWindow = new ProfileViewWindow(this.currentBook.UserId, this.userId);
                NavigationManager.NavigateTo(profileWindow, this);
            }
        }

        /// <summary>
        /// Handles the click event of the "Back" button.
        /// Closes the current window.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Routed event data.</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.GoBack();
        }

        /// <summary>
        /// Handles the click event of the "Home" button.
        /// Navigates to the main page and closes the current window.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Routed event data.</param>
        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.GoToMainPage(this.userId);
        }

        /// <summary>
        /// Handles the click event of the "Report Advertisement" button.
        /// Opens a window for submitting a complaint about the advertisement.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Routed event data.</param>
        private void ReportAd_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentBook == null)
            {
                return;
            }

            AppLogger.Info($"Відкрито вікно скарги на оголошення: BookId={this.currentBook.Id}, UserId={this.userId}");

            var reportWindow = new ReportAdWindow(this.currentBook.Id, this.userId);
            NavigationManager.ShowDialog(reportWindow, this);
        }
    }
}