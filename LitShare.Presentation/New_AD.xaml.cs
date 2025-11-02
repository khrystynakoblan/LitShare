using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LitShare.DAL;
using LitShare.DAL.Models;

namespace LitShare
{
    public partial class NewAdWindow : Window
    {
        private readonly LitShareDbContext _context;

        public NewAdWindow()
        {
            InitializeComponent();
            _context = new LitShareDbContext();
            Loaded += NewAdWindow_Loaded;
        }

        // Завантаження жанрів із бази
        private void NewAdWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var genres = _context.genres.ToList();
                GenreComboBox.ItemsSource = genres;
                GenreComboBox.DisplayMemberPath = "name";
                GenreComboBox.SelectedValuePath = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження жанрів: {ex.Message}");
            }
        }

        // Додавання оголошення
        private async void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AuthorTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                    DealTypeComboBox.SelectedItem == null ||
                    GenreComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Будь ласка, заповніть всі поля перед додаванням!");
                    return;
                }


                var post = new Posts
                {
                    user_id = 1,
                    title = TitleTextBox.Text.Trim(),
                    author = AuthorTextBox.Text.Trim(),
                    deal_type = (DealTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    description = DescriptionTextBox.Text.Trim(),
                    photo_url = "default.jpg"
                };

                _context.posts.Add(post);
                await _context.SaveChangesAsync(); // зберігаємо, щоб отримати post.id


                var selectedGenreId = (int)GenreComboBox.SelectedValue;

                var bookGenre = new BookGenres
                {
                    post_id = post.id,
                    genre_id = selectedGenreId
                };

                _context.bookGenres.Add(bookGenre);
                await _context.SaveChangesAsync();

                MessageBox.Show("Оголошення успішно додано!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
