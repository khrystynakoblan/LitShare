// -----------------------------------------------------------------------
// <copyright file="EditAdWindow.xaml.cs" company="LitShare">
// Copyright (c) 2025 LitShare. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using LitShare.Presentation; // Додано, якщо MainPage знаходиться тут
    using Microsoft.Win32;

    /// <summary>
    /// Логіка взаємодії для вікна редагування оголошення.
    /// </summary>
    public partial class EditAdWindow : Window
    {
        private readonly LitShareDbContext _context;
        private readonly int _postId;
        private readonly int _userId;

        // Виправлення CS8618: nullable поле, бо ініціалізується в LoadPostData
        private Posts? _currentPost;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditAdWindow"/> class.
        /// </summary>
        /// <param name="postId">Ідентифікатор оголошення.</param>
        /// <param name="userId">Ідентифікатор користувача.</param>
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
            var genres = _context.Genres.ToList();
            GenreComboBox.ItemsSource = genres;
            GenreComboBox.DisplayMemberPath = "Name"; // Переконайтесь, що у моделі Genres властивість Name з великої літери
            GenreComboBox.SelectedValuePath = "Id";
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
            _currentPost = _context.Posts.FirstOrDefault(p => p.Id == _postId);

            if (_currentPost == null)
            {
                MessageBox.Show("Оголошення не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            TitleTextBox.Text = _currentPost.Title;
            AuthorTextBox.Text = _currentPost.Author;
            DescriptionTextBox.Text = _currentPost.Description;
            DealTypeComboBox.SelectedValue = _currentPost.DealType;

            var bookGenre = _context.BookGenres.FirstOrDefault(bg => bg.PostId == _postId);
            if (bookGenre != null)
            {
                GenreComboBox.SelectedValue = bookGenre.GenreId;
            }

            if (!string.IsNullOrEmpty(_currentPost.PhotoUrl))
            {
                try
                {
                    BookImage.Source = new BitmapImage(new Uri(_currentPost.PhotoUrl, UriKind.Absolute));
                }
                catch
                {
                    // Ігноруємо помилки відображення старого фото
                }
            }
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPost == null)
            {
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;

                try
                {
                    BookImage.Source = new BitmapImage(new Uri(selectedFile));
                    _currentPost.PhotoUrl = selectedFile;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося завантажити фото: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            try
            {
                if (_currentPost == null)
                {
                    MessageBox.Show("Дані втрачено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _currentPost.Title = TitleTextBox.Text;
                _currentPost.Author = AuthorTextBox.Text;
                _currentPost.Description = DescriptionTextBox.Text;

                if (DealTypeComboBox.SelectedValue is DealType selectedDealType)
                {
                    _currentPost.DealType = selectedDealType;
                }

                var existingGenre = _context.BookGenres.FirstOrDefault(bg => bg.PostId == _postId);

                if (GenreComboBox.SelectedValue is int selectedGenreId)
                {
                    if (existingGenre != null)
                    {
                        if (existingGenre.GenreId != selectedGenreId)
                        {
                            _context.BookGenres.Remove(existingGenre);
                            _context.SaveChanges();

                            _context.BookGenres.Add(new BookGenres
                            {
                                PostId = _postId,
                                GenreId = selectedGenreId
                            });
                        }
                    }
                    else
                    {
                        _context.BookGenres.Add(new BookGenres
                        {
                            PostId = _postId,
                            GenreId = selectedGenreId
                        });
                    }
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
            Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // MainPage може бути в LitShare.Presentation
            var mainWindow = new MainPage(_userId);
            mainWindow.Show();
            Close();
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

        private void Field_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

                if (textBox == TitleTextBox)
                {
                    TitleError.Visibility = Visibility.Collapsed;
                }
                else if (textBox == AuthorTextBox)
                {
                    AuthorError.Visibility = Visibility.Collapsed;
                }
                else if (textBox == DescriptionTextBox)
                {
                    DescriptionError.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

                if (comboBox == GenreComboBox)
                {
                    GenreError.Visibility = Visibility.Collapsed;
                }
                else if (comboBox == DealTypeComboBox)
                {
                    DealTypeError.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}