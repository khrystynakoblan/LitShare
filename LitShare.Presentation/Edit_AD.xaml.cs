// <copyright file="Edit_AD.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using LitShare.Presentation;
    using Microsoft.Win32;

    public partial class EditAdWindow : Window
    {
        private readonly LitShareDbContext context;
        private readonly int postId;
        private Posts currentPost;
        private readonly int userId;

        public EditAdWindow(int postId, int userId)
        {
            this.InitializeComponent();
            this.context = new LitShareDbContext();

            this.postId = postId;
            this.userId = userId;

            this.LoadGenres();
            this.LoadDealTypes();
            this.LoadPostData();
        }


        private void LoadGenres()
        {
            var genres = this.context.Genres.ToList();
            this.GenreComboBox.ItemsSource = genres;
            this.GenreComboBox.DisplayMemberPath = "name";
            this.GenreComboBox.SelectedValuePath = "id";
        }

        private void LoadDealTypes()
        {
            var dealTypes = new[]
            {
                new { Value = DealType.Exchange, Display = "Обмін" },
                new { Value = DealType.Donation, Display = "Безкоштовно" },
            };

            this.DealTypeComboBox.ItemsSource = dealTypes;
            this.DealTypeComboBox.DisplayMemberPath = "Display";
            this.DealTypeComboBox.SelectedValuePath = "Value";
        }


        private void LoadPostData()
        {
            this.currentPost = this.context.Posts.FirstOrDefault(p => p.Id == postId);

            if (this.currentPost == null)
            {
                MessageBox.Show("Оголошення не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            this.TitleTextBox.Text = this.currentPost.Title;
            this.AuthorTextBox.Text = this.currentPost.Author;
            this.DescriptionTextBox.Text = this.currentPost.Description;
            this.DealTypeComboBox.SelectedValue = this.currentPost.DealType;


            var bookGenre = this.context.BookGenres.FirstOrDefault(bg => bg.PostId == this.postId);
            if (bookGenre != null)
            {
                this.GenreComboBox.SelectedValue = bookGenre.GenreId;
            }

            if (!string.IsNullOrEmpty(this.currentPost.PhotoUrl))
            {
                try
                {
                    this.BookImage.Source = new BitmapImage(new Uri(this.currentPost.PhotoUrl, UriKind.Absolute));
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
                Filter = "Зображення (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;

                this.BookImage.Source = new BitmapImage(new Uri(selectedFile));

                // Зберігаємо шлях до фото (локальний шлях)
                currentPost.PhotoUrl = selectedFile;
            }
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                if (currentPost == null)
                {
                    MessageBox.Show("Оголошення не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                currentPost.Title = TitleTextBox.Text;
                currentPost.Author = AuthorTextBox.Text;
                currentPost.Description = DescriptionTextBox.Text;
                currentPost.DealType = (DealType)DealTypeComboBox.SelectedValue;

                var existingGenre = context.BookGenres.FirstOrDefault(bg => bg.PostId == postId);
                int selectedGenreId = (int)GenreComboBox.SelectedValue;

                if (existingGenre != null)
                {
                    if (existingGenre.GenreId != selectedGenreId)
                    {
                        context.BookGenres.Remove(existingGenre);
                        context.SaveChanges();

                        context.BookGenres.Add(new BookGenres
                        {
                            PostId = postId,
                            GenreId = selectedGenreId
                        });
                    }
                }
                else
                {
                    context.BookGenres.Add(new BookGenres
                    {
                        PostId = postId,
                        GenreId = selectedGenreId
                    });
                }

                this.context.SaveChanges();

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
            MainPage mainWindow = new MainPage(userId);
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
