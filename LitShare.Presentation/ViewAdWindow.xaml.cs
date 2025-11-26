// <copyright file="ViewAdWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System.Windows;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;

    /// <summary>
    /// Логіка взаємодії для ViewAdWindow.xaml, що відображає деталі оголошення про книгу.
    /// </summary>
    public partial class ViewAdWindow : Window
    {
        /// <summary>
        /// Сервіс для роботи з даними книг.
        /// </summary>
        private readonly BookService bookService = new ();

        /// <summary>
        /// Ідентифікатор поточного користувача.
        /// </summary>
        private readonly int userId;

        /// <summary>
        /// Дані про книгу, які відображаються у вікні.
        /// </summary>
        private BookDto? currentBook;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAdWindow"/> class.
        /// Ініціалізує новий екземпляр класу <see cref="ViewAdWindow"/>.
        /// Використовується для дизайну або при виклику з іншого конструктора.
        /// </summary>
        public ViewAdWindow()
        {
            this.InitializeComponent();
            this.SetPlaceholder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAdWindow"/> class.
        /// Ініціалізує новий екземпляр класу <see cref="ViewAdWindow"/> з вказаними ідентифікаторами книги та користувача.
        /// </summary>
        /// <param name="bookId">Ідентифікатор книги для відображення.</param>
        /// <param name="userId">Ідентифікатор поточного користувача.</param>
        public ViewAdWindow(int bookId, int userId)
            : this()
        {
            _ = this.LoadBook(bookId);
            this.userId = userId;
        }

        /// <summary>
        /// Встановлює тестові дані-заповнювачі у поля відображення.
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
        /// Асинхронно завантажує дані книги за її ідентифікатором і відображає їх.
        /// </summary>
        /// <param name="bookId">Ідентифікатор книги.</param>
        /// <returns>Задача, що представляє асинхронну операцію.</returns>
        private async Task LoadBook(int bookId)
        {
            try
            {
                this.currentBook = await this.bookService.GetBookById(bookId);
            }
            catch (Exception ex)
            {
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
                MessageBox.Show(
                    "Книга не знайдена.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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

        /// <summary>
        /// Завантажує зображення книги з вказаного URL-шляху.
        /// </summary>
        /// <param name="url">Шлях до зображення (локальний або абсолютний).</param>
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
                // У разі помилки завантаження зображення (наприклад, невірний шлях), просто скидаємо джерело.
                this.PostImage.Source = null;
            }
        }

        /// <summary>
        /// Обробник події натискання на кнопку "Профіль користувача".
        /// Відкриває вікно профілю користувача, який розмістив оголошення.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Дані події маршрутизації.</param>
        private void UserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentBook != null)
            {
                var profileWindow = new ProfileViewWindow(this.currentBook.UserId, this.userId)
                {
                    Owner = this,
                };
                profileWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Обробник події натискання на кнопку "Назад".
        /// Закриває поточне вікно.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Дані події маршрутизації.</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Обробник події натискання на кнопку "Головна сторінка".
        /// Відкриває головну сторінку і закриває поточне вікно.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Дані події маршрутизації.</param>
        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(this.userId);
            mainPage.Show();
            this.Close();
        }

        /// <summary>
        /// Обробник події натискання на кнопку "Поскаржитися на оголошення".
        /// Відкриває вікно для створення скарги.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Дані події маршрутизації.</param>
        private void ReportAd_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentBook == null)
            {
                return;
            }

            this.Hide();

            var reportWindow = new ReportAdWindow(this.currentBook.Id, this.userId)
            {
                Owner = this,
            };
            reportWindow.ShowDialog();

            this.Show();
        }
    }
}