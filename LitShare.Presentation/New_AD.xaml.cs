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
        public NewAdWindow()
        {
            InitializeComponent();
            LoadDealTypes();
            LoadGenres();
        }


        private void LoadDealTypes()
        {
            DealTypeComboBox.ItemsSource = Enum.GetValues(typeof(DealType)).Cast<DealType>();
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
                    deal_type = (DealType)DealTypeComboBox.SelectedItem,
                    user_id = 1,   // тимчасово
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

        private string selectedPhotoPath; 

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {           
            MainWindow mainWindow = new MainWindow();
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



