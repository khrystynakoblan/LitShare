using LitShare.DAL;
using LitShare.DAL.Models;
using LitShare.Presentation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
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
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                string.IsNullOrWhiteSpace(AuthorTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                DealTypeComboBox.SelectedItem == null ||
                GenreComboBox.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!");
                return;
            }

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

                MessageBox.Show("Оголошення успішно додано!");

                this.DialogResult = true;
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
    }
}



