using LitShare.BLL.DTOs;
using LitShare.BLL.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class ViewAdWindow : Window
    {
        private readonly BookService _bookService = new BookService();
        private readonly int _userId;
        private BookDto? _currentBook;

        public ViewAdWindow()
        {
            InitializeComponent();
            SetPlaceholder();
        }

        public ViewAdWindow(int bookId, int userId) : this()
        {
            _ = LoadBook(bookId);
            _userId = userId;
        }

        private void SetPlaceholder()
        {
            TitleText.Text = "Назва (тест)";
            AuthorText.Text = "Автор (тест)";
            DescriptionText.Text = "Опис (тест)";
            DealTypeText.Text = "Тип угоди (тест)";
            LocationText.Text = "Місто (тест)";
            PostImage.Source = null;
        }

        private async Task LoadBook(int bookId)
        {
            try
            {
                _currentBook = await _bookService.GetBookById(bookId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при зверненні до бази: {ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (_currentBook == null)
            {
                MessageBox.Show("Книга не знайдена.", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void LoadBookImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                PostImage.Source = null;
                return;
            }

            try
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                PostImage.Source = img;
            }
            catch
            {
                PostImage.Source = null;
            }
        }


        private void UserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook != null)
            {
                var profileWindow = new ProfileViewWindow(_currentBook.UserId, _userId);
                profileWindow.Owner = this;
                profileWindow.ShowDialog();
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(_userId);
            mainPage.Show();
            this.Close();
        }

        private void ReportAd_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null) return;

            Hide();

            var reportWindow = new ReportAdWindow(_currentBook.Id, _userId);
            reportWindow.Owner = this;
            reportWindow.ShowDialog();

            Show();
        }
    }
}
