using LitShare.BLL.Services;
using LitShare.DAL.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class ViewAdWindow : Window
    {
        private readonly PostService _postService = new PostService();
        private Posts? _currentPost;

        // Конструктор без параметрів для WPF/Dизайнера
        public ViewAdWindow()
        {
            InitializeComponent();

            // Тимчасово можна завантажити "порожнє" або тестове оголошення
            TitleText.Text = "Назва (тест)";
            AuthorText.Text = "Автор (тест)";
            DescriptionText.Text = "Lorem ipsum...";
            DealTypeText.Text = "Тип угоди (тест)";
            PostImage.Source = null;
        }

        // Основний конструктор із postId
        public ViewAdWindow(int postId) : this() // Викликаємо default constructor, щоб InitializeComponent вже був
        {
            try
            {
                LoadPost(postId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження оголошення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadPost(int postId)
        {
            try
            {
                _currentPost = _postService.GetPostById(postId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при зверненні до БД: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (_currentPost == null)
            {
                MessageBox.Show("Оголошення не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            TitleText.Text = _currentPost.title;
            AuthorText.Text = _currentPost.author;
            DescriptionText.Text = _currentPost.description;
            DealTypeText.Text = _currentPost.deal_type;

            if (!string.IsNullOrWhiteSpace(_currentPost.photo_url))
            {
                try
                {
                    Uri uri;
                    if (Uri.TryCreate(_currentPost.photo_url, UriKind.RelativeOrAbsolute, out uri))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = uri;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        PostImage.Source = bitmap;
                    }
                }
                catch
                {
                    PostImage.Source = null;
                }
            }
        }

        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Відкриття вашого профілю...");
        }

        private void UserProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPost != null)
            {
                MessageBox.Show($"Профіль користувача ID: {_currentPost.user_id}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReportAd_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPost != null)
            {
                this.Hide(); // ховаємо ViewAdWindow, але не закриваємо його

                var reportWindow = new ReportAdWindow(_currentPost.id);
                reportWindow.Owner = this; // робимо батьківське вікно
                reportWindow.ShowDialog(); // модально — поки ReportAdWindow відкритий, користувач не зможе взаємодіяти з ViewAdWindow

                this.Show(); // після закриття ReportAdWindow показуємо ViewAdWindow знову
            }
        }


    }
}
