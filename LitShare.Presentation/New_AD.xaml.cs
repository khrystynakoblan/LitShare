// <copyright file="New_AD.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.Logging;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using LitShare.Presentation;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for NewAdWindow.xaml, allowing the user to create a new book advertisement.
    /// </summary>
    public partial class NewAdWindow : Window
    {
        /// <summary>
        /// The ID of the current user.
        /// </summary>
        private readonly int userId;

        /// <summary>
        /// The file path of the selected book photo.
        /// </summary>
        private string? selectedPhotoPath;

        public event Action? AdCreated;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewAdWindow"/> class.
        /// </summary>
        /// <param name="userId">The ID of the user creating the advertisement.</param>
        public NewAdWindow(int userId)
        {
            this.InitializeComponent();
            this.userId = userId;
            AppLogger.Info($"Відкрито NewAdWindow для користувача Id={userId}");

            try
            {
                this.LoadDealTypes();
                this.LoadGenres();
                AppLogger.Info($"Завантажено deal types і genres для користувача Id={userId}");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Помилка при завантаженні deal types і genres", ex);
                this.Close();
            }
        }

        /// <summary>
        /// Loads the available deal types (Exchange, Donation) into the corresponding ComboBox.
        /// </summary>
        private void LoadDealTypes()
        {
            try
            {
                var dealTypes = new[]
                {
                    new { Value = DealType.Exchange, Display = "Обмін" },
                    new { Value = DealType.Donation, Display = "Безкоштовно" },
                };

                this.DealTypeComboBox.ItemsSource = dealTypes;
                this.DealTypeComboBox.DisplayMemberPath = "Display";
                this.DealTypeComboBox.SelectedValuePath = "Value";

                AppLogger.Info("Deal types успішно завантажено");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Помилка при завантаженні deal types", ex);
            }
        }


        /// <summary>
        /// Loads the list of available genres from the database into the corresponding ComboBox.
        /// </summary>
        private void LoadGenres()
        {
            try
        {
                using var db = new LitShareDbContext();
                var genres = db.Genres.ToList();
                this.GenreComboBox.ItemsSource = genres;
                this.GenreComboBox.DisplayMemberPath = "Name";
                this.GenreComboBox.SelectedValuePath = "Id";

                AppLogger.Info("Genres успішно завантажено");
            }
        catch (Exception ex)
        {
                AppLogger.Error("Помилка при завантаженні genres", ex);
            }
        }

        /// <summary>
        /// Handles the click event for the "Add Ad" button.
        /// Saves the new advertisement and associated genre to the database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ValidateFields())
            {
                AppLogger.Warn($"Користувач Id={this.userId} намагався додати оголошення з невалідними полями");
                return;
            }

            try
            {
                using var db = new LitShareDbContext();

                var post = new Posts
                {
                    Title = this.TitleTextBox.Text,
                    Author = this.AuthorTextBox.Text,
                    Description = this.DescriptionTextBox.Text,
                    DealType = (DealType)this.DealTypeComboBox.SelectedValue,
                    UserId = this.userId,
                    PhotoUrl = this.selectedPhotoPath,
                    User = null,
                };

                db.Posts.Add(post);
                db.SaveChanges();

                var selectedGenre = (Genres)this.GenreComboBox.SelectedItem;
                var bookGenre = new BookGenres
                {
                    PostId = post.Id,
                    GenreId = selectedGenre.Id,
                };

                db.BookGenres.Add(bookGenre);
                db.SaveChanges();

                AdCreated?.Invoke(); // <<<<<< ОНОВИТИ ГОЛОВНЕ ВІКНО


                AppLogger.Info($"Користувач Id={this.userId} додав нове оголошення Id={post.Id}, жанр Id={selectedGenre.Id}");

                // Navigate to the main page and scroll to the new ad
                var mainWindow = new MainPage(this.userId);
                mainWindow.Loaded += (s, e2) => mainWindow.ScrollToBottom();

                this.Close();
            }
            catch (DbUpdateException ex)
            {
                AppLogger.Error("Помилка при збереженні нового оголошення в БД", ex);
                MessageBox.Show(
                    $"An error occurred while adding the advertisement:\n{ex.InnerException?.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Невідома помилка при додаванні оголошення", ex);
                MessageBox.Show($"An unknown error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event for the "Cancel" button.
        /// Closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Користувач Id={this.userId} закрив NewAdWindow без додавання оголошення");
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the "My Profile" button.
        /// Opens the user's profile window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Користувач Id={this.userId} відкрив ProfileWindow з NewAdWindow");
            var profileWindow = new ProfileWindow(this.userId);
            profileWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the click event for the "Home" button.
        /// Opens the main page and closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Користувач Id={this.userId} перейшов на MainPage з NewAdWindow");
            MainPage mainWindow = new (this.userId);
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the "Add Photo" button.
        /// Opens a file dialog to select an image file and displays it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Book Photo",
                    Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    Multiselect = false,
                };

                bool? result = openFileDialog.ShowDialog();

                if (result == true)
                {
                    this.selectedPhotoPath = openFileDialog.FileName;

                    if (this.BookImage != null)
                    {
                        this.BookImage.Source = new BitmapImage(new Uri(this.selectedPhotoPath));
                    }

                    AppLogger.Info($"Користувач Id={this.userId} вибрав фото: {this.selectedPhotoPath}");
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Помилка при виборі фото", ex);
            }
        }

        /// <summary>
        /// Validates if all required fields in the form are filled and displays error messages if necessary.
        /// </summary>
        /// <returns><c>true</c> if all fields are valid; otherwise, <c>false</c>.</returns>
        private bool ValidateFields()
        {
            bool isValid = true;
            var redBrush = new SolidColorBrush(Colors.Red);
            var normalBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179)); // Standard Border Color (Gray)

            // Title Validation
            if (string.IsNullOrWhiteSpace(this.TitleTextBox.Text))
            {
                this.TitleTextBox.BorderBrush = redBrush;
                this.TitleError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                this.TitleTextBox.BorderBrush = normalBrush;
                this.TitleError.Visibility = Visibility.Collapsed;
            }

            // Author Validation
            if (string.IsNullOrWhiteSpace(this.AuthorTextBox.Text))
            {
                this.AuthorTextBox.BorderBrush = redBrush;
                this.AuthorError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                this.AuthorTextBox.BorderBrush = normalBrush;
                this.AuthorError.Visibility = Visibility.Collapsed;
            }

            // Description Validation
            if (string.IsNullOrWhiteSpace(this.DescriptionTextBox.Text))
            {
                this.DescriptionTextBox.BorderBrush = redBrush;
                this.DescriptionError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                this.DescriptionTextBox.BorderBrush = normalBrush;
                this.DescriptionError.Visibility = Visibility.Collapsed;
            }

            // DealType ComboBox Validation
            if (this.DealTypeComboBox.SelectedItem == null)
            {
                this.DealTypeComboBox.BorderBrush = redBrush;
                this.DealTypeError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                this.DealTypeComboBox.BorderBrush = normalBrush;
                this.DealTypeError.Visibility = Visibility.Collapsed;
            }

            // Genre ComboBox Validation
            if (this.GenreComboBox.SelectedItem == null)
            {
                this.GenreComboBox.BorderBrush = redBrush;
                this.GenreError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                this.GenreComboBox.BorderBrush = normalBrush;
                this.GenreError.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }

        /// <summary>
        /// Handles the text changed event, resetting the border color and hiding the error message for the TextBox.
        /// </summary>
        /// <param name="sender">The TextBox where the text was changed.</param>
        /// <param name="e">The text changed event data.</param>
        private void Field_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

                if (textBox == this.TitleTextBox)
                {
                    this.TitleError.Visibility = Visibility.Collapsed;
                }
                else if (textBox == this.AuthorTextBox)
                {
                    this.AuthorError.Visibility = Visibility.Collapsed;
                }
                else if (textBox == this.DescriptionTextBox)
                {
                    this.DescriptionError.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handles the selection changed event in a ComboBox, resetting the border color and hiding the error message.
        /// </summary>
        /// <param name="sender">The ComboBox where the selection was changed.</param>
        /// <param name="e">The selection changed event data.</param>
        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

                if (comboBox == this.GenreComboBox)
                {
                    this.GenreError.Visibility = Visibility.Collapsed;
                }
                else if (comboBox == this.DealTypeComboBox)
                {
                    this.DealTypeError.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}