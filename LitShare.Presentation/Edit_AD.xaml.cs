using LitShare.DAL;
using LitShare.DAL.Models;
using LitShare.Presentation;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LitShare
{
    public partial class EditAdWindow : Window
    {
        private readonly LitShareDbContext _context;
        private readonly int _postId;
        private Posts _currentPost;
        private readonly int _userId;

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


        private void LoadGenres()
        {
            var genres = _context.genres.ToList();
            GenreComboBox.ItemsSource = genres;
            GenreComboBox.DisplayMemberPath = "name";
            GenreComboBox.SelectedValuePath = "id";
        }

        private void LoadDealTypes()
        {
            var dealTypes = new[]
            {
                new { Value = DealType.Exchange, Display = "Обмін" },
                new { Value = DealType.Donation, Display = "Безкоштовно" }
            };

            DealTypeComboBox.ItemsSource = dealTypes;
            DealTypeComboBox.DisplayMemberPath = "Display";
            DealTypeComboBox.SelectedValuePath = "Value";
        }


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
            DealTypeComboBox.SelectedValue = _currentPost.deal_type;


            var bookGenre = _context.bookGenres.FirstOrDefault(bg => bg.post_id == _postId);
            if (bookGenre != null)
            {
                GenreComboBox.SelectedValue = bookGenre.genre_id;
            }

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

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;

                BookImage.Source = new BitmapImage(new Uri(selectedFile));

                // Зберігаємо шлях до фото (локальний шлях)
                _currentPost.photo_url = selectedFile;
            }
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

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
                _currentPost.deal_type = (DealType)DealTypeComboBox.SelectedValue;

                var existingGenre = _context.bookGenres.FirstOrDefault(bg => bg.post_id == _postId);
                int selectedGenreId = (int)GenreComboBox.SelectedValue;

                if (existingGenre != null)
                {
                    if (existingGenre.genre_id != selectedGenreId)
                    {
                        _context.bookGenres.Remove(existingGenre);
                        _context.SaveChanges();

                        _context.bookGenres.Add(new BookGenres
                        {
                            post_id = _postId,
                            genre_id = selectedGenreId
                        });
                    }
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

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка при збереженні змін:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainWindow = new MainPage(_userId);
            mainWindow.Show();
            this.Close();
        }

        private bool ValidateFields()
        {
            bool isValid = true;
            var redBrush = new SolidColorBrush(Colors.Red);
            var normalBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                TitleTextBox.BorderBrush = redBrush;
                TitleError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                TitleTextBox.BorderBrush = normalBrush;
                TitleError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(AuthorTextBox.Text))
            {
                AuthorTextBox.BorderBrush = redBrush;
                AuthorError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                AuthorTextBox.BorderBrush = normalBrush;
                AuthorError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                DescriptionTextBox.BorderBrush = redBrush;
                DescriptionError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                DescriptionTextBox.BorderBrush = normalBrush;
                DescriptionError.Visibility = Visibility.Collapsed;
            }

            if (DealTypeComboBox.SelectedItem == null)
            {
                DealTypeComboBox.BorderBrush = redBrush;
                DealTypeError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                DealTypeComboBox.BorderBrush = normalBrush;
                DealTypeError.Visibility = Visibility.Collapsed;
            }

            if (GenreComboBox.SelectedItem == null)
            {
                GenreComboBox.BorderBrush = redBrush;
                GenreError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                GenreComboBox.BorderBrush = normalBrush;
                GenreError.Visibility = Visibility.Collapsed;
            }



            return isValid;
        }

        private void Field_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

            if (textBox == TitleTextBox) TitleError.Visibility = Visibility.Collapsed;
            else if (textBox == AuthorTextBox) AuthorError.Visibility = Visibility.Collapsed;
            else if (textBox == DescriptionTextBox) DescriptionError.Visibility = Visibility.Collapsed;
        }

        private void Field_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

            if (comboBox == GenreComboBox) GenreError.Visibility = Visibility.Collapsed;
            else if (comboBox == DealTypeComboBox) DealTypeError.Visibility = Visibility.Collapsed;
        }
    }
}
