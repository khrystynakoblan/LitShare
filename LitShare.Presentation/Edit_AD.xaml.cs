// <copyright file="Edit_AD.xaml.cs" company="PlaceholderCompany">
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
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using LitShare.Presentation;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for EditAdWindow.xaml.
    /// This window allows a user to edit the details of an existing book advertisement.
    /// </summary>
    public partial class EditAdWindow : Window
    {
        /// <summary>
        /// The database context for data access. NOTE: This is tightly coupled and should ideally be managed by dependency injection.
        /// </summary>
        private readonly LitShareDbContext context;

        /// <summary>
        /// The ID of the post (advertisement) being edited.
        /// </summary>
        private readonly int postId;

        /// <summary>
        /// The ID of the current user.
        /// </summary>
        private readonly int userId;

        /// <summary>
        /// The data model representing the current post being edited.
        /// </summary>
        private Posts? currentPost;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditAdWindow"/> class.
        /// </summary>
        /// <param name="postId">The ID of the advertisement to load and edit.</param>
        /// <param name="userId">The ID of the user performing the edit.</param>
        public EditAdWindow(int postId, int userId)
        {
            this.InitializeComponent();
            this.context = new LitShareDbContext();
            this.postId = postId;
            this.userId = userId;

            this.LoadData();
        }

        /// <summary>
        /// Loads all necessary data (genres, deal types, and existing post data) into the form.
        /// </summary>
        private void LoadData()
        {
            try
            {
                this.LoadGenres();
                this.LoadDealTypes();
                this.LoadPostData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Loads all available genres from the database into the Genre ComboBox.
        /// </summary>
        private void LoadGenres()
        {
            var genres = this.context.Genres.ToList();
            this.GenreComboBox.ItemsSource = genres;
            this.GenreComboBox.DisplayMemberPath = "name";
            this.GenreComboBox.SelectedValuePath = "id";
        }

        /// <summary>
        /// Loads the predefined deal types (Exchange, Donation) into the DealType ComboBox.
        /// </summary>
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

        /// <summary>
        /// Retrieves the existing post data from the database and populates the form controls.
        /// </summary>
        private void LoadPostData()
        {
            this.currentPost = this.context.Posts.FirstOrDefault(p => p.Id == this.postId);

            if (this.currentPost == null)
            {
                MessageBox.Show("Advertisement not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Populate text fields and combo box for post data
            this.TitleTextBox.Text = this.currentPost.Title;
            this.AuthorTextBox.Text = this.currentPost.Author;
            this.DescriptionTextBox.Text = this.currentPost.Description;
            this.DealTypeComboBox.SelectedValue = this.currentPost.DealType;

            // Load genre data
            var bookGenre = this.context.BookGenres.FirstOrDefault(bg => bg.PostId == this.postId);
            if (bookGenre != null)
            {
                this.GenreComboBox.SelectedValue = bookGenre.GenreId;
            }

            // Load and display the existing image
            if (!string.IsNullOrEmpty(this.currentPost.PhotoUrl))
            {
                try
                {
                    this.BookImage.Source = new BitmapImage(new Uri(this.currentPost.PhotoUrl, UriKind.Absolute));
                }
                catch (UriFormatException)
                {
                    // Handle image loading error, e.g., if the URL is invalid
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Handle case where local file is missing
                }
            }
        }

        /// <summary>
        /// Handles the click event for the "Add Photo" button.
        /// Opens a file dialog and updates the image display and the post's PhotoUrl field.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Images (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;

                try
                {
                    this.BookImage.Source = new BitmapImage(new Uri(selectedFile));
                    if (this.currentPost != null)
                    {
                        this.currentPost.PhotoUrl = selectedFile;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image:\n{ex.Message}", "Image Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Handles the click event for the "Save Ad" button (originally AddAdButton).
        /// Validates fields, updates the post and genre data, and saves changes to the database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.ValidateFields())
            {
                return;
            }

            if (this.currentPost == null)
            {
                MessageBox.Show("Advertisement not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                this.UpdatePostData();
                this.UpdateBookGenre();

                this.context.SaveChanges();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the properties of the <see cref="currentPost"/> object with data from the form controls.
        /// </summary>
        private void UpdatePostData()
        {
            if (this.currentPost != null)
            {
                this.currentPost.Title = this.TitleTextBox.Text;
                this.currentPost.Author = this.AuthorTextBox.Text;
                this.currentPost.Description = this.DescriptionTextBox.Text;
                this.currentPost.DealType = (DealType)(this.DealTypeComboBox.SelectedValue ?? DealType.Donation);
            }
        }

        /// <summary>
        /// Updates the associated genre for the post in the BookGenres table.
        /// </summary>
        private void UpdateBookGenre()
        {
            if (this.GenreComboBox.SelectedValue is int selectedGenreId)
            {
                var existingGenre = this.context.BookGenres.FirstOrDefault(bg => bg.PostId == this.postId);

                if (existingGenre != null)
                {
                    if (existingGenre.GenreId != selectedGenreId)
                    {
                        this.context.BookGenres.Remove(existingGenre);
                        this.context.SaveChanges(); // Necessary if FK constraints require immediate deletion

                        this.context.BookGenres.Add(new BookGenres
                        {
                            PostId = this.postId,
                            GenreId = selectedGenreId,
                        });
                    }
                }
                else
                {
                    this.context.BookGenres.Add(new BookGenres
                    {
                        PostId = this.postId,
                        GenreId = selectedGenreId,
                    });
                }
            }
        }

        /// <summary>
        /// Handles the click event for the "Cancel" button.
        /// Closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();

        /// <summary>
        /// Handles the click event for the "Home" button.
        /// Navigates back to the main page and closes the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The routing event data.</param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to main page
            MainPage mainWindow = new MainPage(this.userId);
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Validates all required input fields and highlights errors.
        /// </summary>
        /// <returns><c>true</c> if all fields are valid; otherwise, <c>false</c>.</returns>
        private bool ValidateFields()
        {
            bool isValid = true;
            var redBrush = new SolidColorBrush(Colors.Red);

            isValid &= this.ValidateTextBox(this.TitleTextBox, this.TitleError, redBrush);
            isValid &= this.ValidateTextBox(this.AuthorTextBox, this.AuthorError, redBrush);
            isValid &= this.ValidateTextBox(this.DescriptionTextBox, this.DescriptionError, redBrush);

            isValid &= this.ValidateComboBox(this.DealTypeComboBox, this.DealTypeError, redBrush);
            isValid &= this.ValidateComboBox(this.GenreComboBox, this.GenreError, redBrush);

            return isValid;
        }

        /// <summary>
        /// Helper method to validate a TextBox control for empty or null content.
        /// </summary>
        /// <param name="textBox">The TextBox control to validate.</param>
        /// <param name="errorControl">The error UI element to show/hide.</param>
        /// <param name="redBrush">The brush for the error border color.</param>
        /// <returns><c>true</c> if valid; otherwise, <c>false</c>.</returns>
        private bool ValidateTextBox(TextBox textBox, FrameworkElement errorControl, SolidColorBrush redBrush)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BorderBrush = redBrush;
                errorControl.Visibility = Visibility.Visible;
                return false;
            }

            textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179)); // Normal color
            errorControl.Visibility = Visibility.Collapsed;
            return true;
        }

        /// <summary>
        /// Helper method to validate a ComboBox control for an unselected item.
        /// </summary>
        /// <param name="comboBox">The ComboBox control to validate.</param>
        /// <param name="errorControl">The error UI element to show/hide.</param>
        /// <param name="redBrush">The brush for the error border color.</param>
        /// <returns><c>true</c> if valid; otherwise, <c>false</c>.</returns>
        private bool ValidateComboBox(ComboBox comboBox, FrameworkElement errorControl, SolidColorBrush redBrush)
        {
            if (comboBox.SelectedItem == null)
            {
                comboBox.BorderBrush = redBrush;
                errorControl.Visibility = Visibility.Visible;
                return false;
            }

            comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179)); // Normal color
            errorControl.Visibility = Visibility.Collapsed;
            return true;
        }

        /// <summary>
        /// Unified event handler for TextChanged and SelectionChanged events to reset errors.
        /// </summary>
        /// <param name="sender">The control that raised the event (TextBox or ComboBox).</param>
        /// <param name="e">The event data.</param>
        private void Field_Changed(object sender, EventArgs e)
        {
            var normalBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

            if (sender is TextBox textBox)
            {
                textBox.BorderBrush = normalBrush;

                // Explicitly check and collapse the relevant error message
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
            else if (sender is ComboBox comboBox)
            {
                comboBox.BorderBrush = normalBrush;

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

        /// <summary>
        /// Event handler for the TextChanged event, redirecting to the unified <see cref="Field_Changed"/> method.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Field_TextChanged(object sender, TextChangedEventArgs e) => this.Field_Changed(sender, e);

        /// <summary>
        /// Event handler for the SelectionChanged event, redirecting to the unified <see cref="Field_Changed"/> method.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Field_SelectionChanged(object sender, SelectionChangedEventArgs e) => this.Field_Changed(sender, e);
    }
}