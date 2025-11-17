// SA1633: Файл заголовка додано.
// <copyright file="ViewAdWindow.xaml.cs" company="LitShare">
// Copyright (c) LitShare. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using LitShare.BLL.DTOs;
using LitShare.BLL.Services;

namespace LitShare.Presentation
{
    // SA1600: Клас має бути задокументований.
    /// <summary>
    /// Логіка взаємодії для ViewAdWindow.xaml.
    /// </summary>
    public partial class ViewAdWindow : Window
    {
        // SA1309: Поля тепер без префікса '_'.
        private readonly BookService bookService = new BookService();
        private readonly int userId;
        private BookDto? currentBook;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="ViewAdWindow"/>.
        /// </summary>
        /// 
        public ViewAdWindow()
        {
            this.InitializeComponent();
            this.SetPlaceholder();
        }

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="ViewAdWindow"/> для відображення конкретної книги.
        /// </summary>
        /// <param name="bookId">Ідентифікатор книги.</param>
        /// <param name="userId">Ідентифікатор поточного користувача.</param>
        public ViewAdWindow(int bookId, int userId)
            : this()
        {
            _ = this.LoadBook(bookId);
            this.userId = userId;
        }

        private void SetPlaceholder()
        {
            this.TitleText.Text = "Назва (тест)";
            this.AuthorText.Text = "Автор (тест)";
            this.DescriptionText.Text = "Опис (тест)";
            this.DealTypeText.Text = "Тип угоди (тест)";
            this.LocationText.Text = "Місто (тест)";
            this.PostImage.Source = null;
        }

        private async Task LoadBook(int bookId)
        {
            try
            {
                this.currentBook = await this.bookService.GetBookById(bookId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при зверненні до бази: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            if (this.currentBook == null)
            {
                MessageBox.Show("Книга не знайдена.", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
                return;
            }

            this.TitleText.Text = this.currentBook.Title ?? string.Empty;
            this.AuthorText.Text = this.currentBook.Author ?? string.Empty;
            this.DescriptionText.Text = this.currentBook.Description ?? string.Empty;
            this.DealTypeText.Text = this.currentBook.DealType ?? string.Empty;
            this.LocationText.Text = this.currentBook.Location ?? string.Empty;

            this.LoadBookImage(this.currentBook.ImagePath);
        }

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
            catch
            {
                this.PostImage.Source = null;
            }
        }

        private void UserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentBook != null)
            {
                var profileWindow = new ProfileViewWindow(this.currentBook.UserId, this.userId);
                profileWindow.Owner = this;
                profileWindow.ShowDialog();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(this.userId);
            mainPage.Show();
            this.Close();
        }

        private void ReportAd_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentBook == null)
            {
                return;
            }

            this.Hide();

            var reportWindow = new ReportAdWindow(this.currentBook.Id, this.userId);
            reportWindow.Owner = this;
            reportWindow.ShowDialog();

            this.Show();
        }
    }
}
