using LitShare.DAL;
using LitShare.DAL.Models;
using LitShare.Presentation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LitShare
{
    public partial class NewAdWindow : Window
    {
        private readonly int _userId;
        public NewAdWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDealTypes();
            LoadGenres();
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


        private void LoadGenres()
        {
            using var db = new LitShareDbContext();
            var genres = db.genres.ToList();
            GenreComboBox.ItemsSource = genres;
            GenreComboBox.DisplayMemberPath = "name";
            GenreComboBox.SelectedValuePath = "id";
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                using var db = new LitShareDbContext();

                var post = new Posts
                {
                    title = TitleTextBox.Text,
                    author = AuthorTextBox.Text,
                    description = DescriptionTextBox.Text,
                    deal_type = (DealType)DealTypeComboBox.SelectedValue,
                    user_id = _userId,
                    photo_url = selectedPhotoPath
                };

                db.posts.Add(post);
                db.SaveChanges();

                var selectedGenre = (Genres)GenreComboBox.SelectedItem;
                var bookGenre = new BookGenres
                {
                    post_id = post.id,
                    genre_id = selectedGenre.id
                };

                db.bookGenres.Add(bookGenre);
                db.SaveChanges();

                var mainWindow = new MainPage(_userId);
                mainWindow.Loaded += (s, e2) => mainWindow.ScrollToBottom();
                mainWindow.Show();

                this.Close();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Сталася помилка при додаванні оголошення:\n{ex.InnerException?.Message}");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_userId);
            profileWindow.ShowDialog();
        }

        private string selectedPhotoPath; 

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainWindow = new MainPage(_userId);
            mainWindow.Show();
            this.Close();
        }
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Виберіть фото книги",
                Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };


            bool? result = openFileDialog.ShowDialog();

                        if (result == true)
                        {
                            selectedPhotoPath = openFileDialog.FileName;

                            if (BookImage != null)
                            {
                                BookImage.Source = new BitmapImage(new Uri(selectedPhotoPath));
                            }

                            MessageBox.Show("Фото вибрано успішно!");
                        }
         }
        private bool ValidateFields()
        {
            bool isValid = true;
            var redBrush = new SolidColorBrush(Colors.Red);
            var normalBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

            // --- Назва ---
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

            // --- Автор ---
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

            // --- Опис ---
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

            // --- Тип угоди ---
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

            // --- Жанр ---
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



