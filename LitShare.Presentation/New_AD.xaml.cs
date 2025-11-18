// -----------------------------------------------------------------------
// <copyright file="NewAdWindow.xaml.cs" company="LitShare">
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
    using LitShare.Presentation;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Win32;

    /// <summary>
    /// Логіка взаємодії для вікна створення нового оголошення.
    /// </summary>
    public partial class NewAdWindow : Window
    {
        private readonly LitShareDbContext _context;
        private readonly int _userId;
        private string _selectedPhotoPath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewAdWindow"/> class.
        /// </summary>
        /// <param name="userId">Ідентифікатор поточного користувача.</param>
        public NewAdWindow(int userId)
        {
            InitializeComponent();
            _context = new LitShareDbContext();
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
            // Використовуємо спільний контекст _context замість створення нового
            var genres = _context.Genres.ToList();
            GenreComboBox.ItemsSource = genres;
            GenreComboBox.DisplayMemberPath = "Name"; // Виправлено регістр (Name)
            GenreComboBox.SelectedValuePath = "Id";   // Виправлено регістр (Id)
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            try
            {
                var post = new Posts
                {
                    Title = TitleTextBox.Text,
                    Author = AuthorTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    DealType = (DealType)DealTypeComboBox.SelectedValue,
                    UserId = _userId,
                    PhotoUrl = string.IsNullOrEmpty(_selectedPhotoPath) ? null : _selectedPhotoPath
                };

                _context.Posts.Add(post);
                _context.SaveChanges(); // Зберігаємо, щоб отримати post.Id

                if (GenreComboBox.SelectedItem is Genres selectedGenre)
                {
                    var bookGenre = new BookGenres
                    {
                        PostId = post.Id,
                        GenreId = selectedGenre.Id
                    };

                    _context.BookGenres.Add(bookGenre);
                    _context.SaveChanges();
                }

                var mainWindow = new MainPage(_userId);

                // Використовуємо лямбда-вираз для прокрутки після завантаження
                mainWindow.Loaded += (s, e2) => mainWindow.ScrollToBottom();

                mainWindow.Show();
                Close();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Сталася помилка при додаванні оголошення:\n{ex.InnerException?.Message ?? ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непередбачена помилка:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_userId);
            profileWindow.ShowDialog();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainPage(_userId);
            mainWindow.Show();
            Close();
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Виберіть фото книги",
                Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                _selectedPhotoPath = openFileDialog.FileName;

                if (BookImage != null)
                {
                    try
                    {
                        BookImage.Source = new BitmapImage(new Uri(_selectedPhotoPath));
                    }
                    catch
                    {
                        // Ігноруємо помилки відображення
                    }
                }
            }
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