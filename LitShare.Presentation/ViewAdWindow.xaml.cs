using LitShare.BLL.DTOs;
using LitShare.BLL.Services;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class ViewAdWindow : Window
    {
        private readonly BookService _bookService = new BookService();
        private PostDto? _currentBook;

        // Конструктор без параметрів для WPF/Dизайнера
        public ViewAdWindow()
        {
            InitializeComponent();
            SetPlaceholder();
        }

        // Конструктор із передачею Id книги
        public ViewAdWindow(int bookId) : this()
        {
            LoadBook(bookId);
        }

        // Тимчасовий placeholder для дизайнера
        private void SetPlaceholder()
        {
            TitleText.Text = "Назва (тест)";
            AuthorText.Text = "Автор (тест)";
            DescriptionText.Text = "Опис (тест)";
            DealTypeText.Text = "Тип угоди (тест)";
            LocationText.Text = "Місто (тест)";
            PostImage.Source = null;
        }

        // Завантаження книги з бази
        private void LoadBook(int bookId)
        {
            try
            {
                _currentBook = _bookService.GetBookById(bookId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при зверненні до БД: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (_currentBook == null)
            {
                MessageBox.Show("Книга не знайдена.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            TitleText.Text = _currentBook.Title ?? "";
            AuthorText.Text = _currentBook.Author ?? "";
            DescriptionText.Text = _currentBook.Description ?? "";
            DealTypeText.Text = _currentBook.DealType ?? "";
            LocationText.Text = _currentBook.Location ?? "";

            LoadBookImage(_currentBook.ImagePath);
        }

        // Завантаження зображення
        private void LoadBookImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                PostImage.Source = null;
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PostImage.Source = bitmap;
            }
            catch
            {
                PostImage.Source = null;
            }
        }

        // Кнопка "Мій профіль"
        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Відкриття вашого профілю...");
        }

        // Кнопка перегляду профілю користувача
        private void UserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook != null)
            {
                MessageBox.Show($"Профіль користувача (тут можна додати user_id якщо є в BookDto)");
            }
        }

        // Кнопка "Назад"
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Кнопка "Скарга на оголошення"
        private void ReportAd_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook != null)
            {
                this.Hide();

                var reportWindow = new ReportAdWindow(_currentBook.Id);
                reportWindow.Owner = this;
                reportWindow.ShowDialog();

                this.Show();
            }
        }
    }
}
