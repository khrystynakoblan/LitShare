using LitShare.DAL;
using LitShare.DAL.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LitShare
{
    public partial class EditAdWindow : Window
    {
        private readonly LitShareDbContext _context;
        private readonly int _postId = 1;
        private Posts _currentPost;
        private readonly int _userId = 1;

        public EditAdWindow()
        {
            InitializeComponent();
            _context = new LitShareDbContext();
            _postId = 1;
            _userId = 1;

            LoadGenres();
            LoadDealTypes();
            LoadPostData();
            MessageBox.Show($"Post ID: {_postId}, Found: {_currentPost != null}");

        }

        public EditAdWindow(int postId, int userId)
        {
            InitializeComponent();
            _context = new LitShareDbContext();
            
            _postId = postId;
            _userId = userId;

            LoadGenres();
            LoadDealTypes();
            LoadPostData();
        }

        // Завантаження жанрів у ComboBox
        private void LoadGenres()
        {
            var genres = _context.genres.ToList();
            GenreComboBox.ItemsSource = genres;
            GenreComboBox.DisplayMemberPath = "name";
            GenreComboBox.SelectedValuePath = "id";
        }

        // Завантаження enum DealType у ComboBox
        private void LoadDealTypes()
        {
            DealTypeComboBox.ItemsSource = Enum.GetValues(typeof(DealType));
        }

        // Завантаження оголошення для редагування
        private void LoadPostData()
        {
            _currentPost = _context.posts.FirstOrDefault(p => p.id == _postId);

            if (_currentPost == null)
            {
                MessageBox.Show("Оголошення не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            TitleTextBox.Text = _currentPost.title;
            AuthorTextBox.Text = _currentPost.author;
            DescriptionTextBox.Text = _currentPost.description;
            DealTypeComboBox.SelectedItem = _currentPost.deal_type;

            // Підтягнути жанр з таблиці зв’язків
            var bookGenre = _context.bookGenres.FirstOrDefault(bg => bg.post_id == _postId);
            if (bookGenre != null)
            {
                GenreComboBox.SelectedValue = bookGenre.genre_id;
            }

            // Завантаження фото (якщо є URL)
            if (!string.IsNullOrEmpty(_currentPost.photo_url))
            {
                try
                {
                    BookImage.Source = new BitmapImage(new Uri(_currentPost.photo_url, UriKind.Absolute));
                }
                catch
                {
                    // Якщо URL некоректний або файл відсутній — ігноруємо
                }
            }
        }

        // Зміна фото
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;

                // Встановлюємо у вікні
                BookImage.Source = new BitmapImage(new Uri(selectedFile));

                // Зберігаємо шлях до фото (у цьому прикладі — локальний шлях)
                _currentPost.photo_url = selectedFile;
            }
        }

        // Збереження змін
        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentPost == null)
                {
                    MessageBox.Show("Оголошення не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _currentPost.title = TitleTextBox.Text;
                _currentPost.author = AuthorTextBox.Text;
                _currentPost.description = DescriptionTextBox.Text;
                _currentPost.deal_type = (DealType)DealTypeComboBox.SelectedItem;

                // Оновити жанр у таблиці book_genres
                var existingGenre = _context.bookGenres.FirstOrDefault(bg => bg.post_id == _postId);
                int selectedGenreId = (int)GenreComboBox.SelectedValue;

                if (existingGenre != null)
                {
                    existingGenre.genre_id = selectedGenreId;
                }
                else
                {
                    _context.bookGenres.Add(new BookGenres
                    {
                        post_id = _postId,
                        genre_id = selectedGenreId
                    });
                }

                _context.SaveChanges();
                MessageBox.Show("Оголошення успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка при збереженні змін:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Відхилити зміни
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Скасувати зміни?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        // Кнопка LitShare → повернення на головну
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
