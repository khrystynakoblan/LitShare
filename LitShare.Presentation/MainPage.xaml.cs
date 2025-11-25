// <copyright file="MainPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;

    /// <summary>
    /// Represents the main window for browsing books and applying filters in the LitShare application.
    /// </summary>
    public partial class MainPage : Window
    {
        /// <summary>
        /// Gets or sets the collection of all books loaded from the database.
        /// </summary>
        public ObservableCollection<BookDto> AllBooks { get; set; } = new ObservableCollection<BookDto>();

        /// <summary>
        /// Gets or sets the collection of books currently displayed after filtering.
        /// </summary>
        public ObservableCollection<BookDto> FilteredBooks { get; set; } = new ObservableCollection<BookDto>();

        private static readonly BookService bookService1 = new BookService();

        // SA1214 - Readonly fields first (SA1309 - fields should not begin with an underscore)
        private readonly BookService bookService = bookService1;
        private readonly int userId;

        // SA1214 - Non-readonly fields
        private bool isSearchPlaceholder = true;
        private readonly List<CheckBox> genreCheckBoxes = new List<CheckBox>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        /// <param name="userId">The ID of the currently logged-in user.</param>
        public MainPage(int userId)
        {
            this.InitializeComponent();
            this.userId = userId; // зберігаємо ID користувача
            this.Loaded += this.MainPage_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the main page, asynchronously loads books and genres, and sets up the UI.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ResultsCountText.Text = "Завантаження даних...";

                var books = await this.bookService.GetAllBooksAsync();
                var genres = await this.bookService.GetGenresAsync();

                // IDE0028 - Collection initialization can be simplified (new(books))
                this.AllBooks = new ObservableCollection<BookDto>(books);
                this.FilteredBooks = new ObservableCollection<BookDto>(books);
                this.BooksItemsControl.ItemsSource = this.FilteredBooks;

                this.GenresPanel.Children.Clear();
                this.genreCheckBoxes.Clear();

                foreach (var genre in genres)
                {
                    var checkBox = new CheckBox
                    {
                        Content = genre,
                        Margin = new Thickness(0, 3, 0, 3),
                    };
                    checkBox.Checked += this.FilterChanged;
                    checkBox.Unchecked += this.FilterChanged;
                    this.GenresPanel.Children.Add(checkBox);
                    this.genreCheckBoxes.Add(checkBox);
                }

                this.UpdateResultsCount();
                this.SetupSearchPlaceholder();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації: {ex.Message}");
            }
        }

        private void FilterChanged(object sender, RoutedEventArgs e) => this.ApplyFilters();

        private List<string> GetSelectedGenres() =>
            this.genreCheckBoxes
                .Where(cb => cb.IsChecked == true && cb.Content != null)
                .Select(cb => cb.Content!.ToString()!)
                .ToList();

        private void ApplyFilters()
        {
            if (this.AllBooks == null)
            {
                return;
            }

            string? location = this.LocationTextBox?.Text?.Trim();
            string? search = this.isSearchPlaceholder ? null : this.SearchTextBox?.Text?.Trim();
            string? dealType = this.ExchangeRadio?.IsChecked == true ? "Обмін" :
                                 this.FreeRadio?.IsChecked == true ? "Безкоштовно" : null;
            var selectedGenres = this.GetSelectedGenres();

            var filtered = this.bookService.GetFilteredBooks(this.AllBooks.ToList(), search, location, dealType, selectedGenres);

            this.FilteredBooks.Clear();
            foreach (var book in filtered)
            {
                this.FilteredBooks.Add(book);
            }

            this.UpdateResultsCount();
        }

        private void SetupSearchPlaceholder()
        {
            this.SearchTextBox.Text = "Пошук...";
            this.SearchTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            this.isSearchPlaceholder = true;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.isSearchPlaceholder)
            {
                // SA1122: Use string.Empty for empty strings
                this.SearchTextBox.Text = string.Empty;
                this.SearchTextBox.Foreground = new SolidColorBrush(Colors.Black);
                this.isSearchPlaceholder = false;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.SearchTextBox.Text))
            {
                this.SetupSearchPlaceholder();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.isSearchPlaceholder)
            {
                this.ApplyFilters();
            }
        }

        /// <summary>
        /// Handles the click event for the MyProfile button, opening the profile window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(this.userId);
            profileWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the click event for the AddBook button, opening the new ad window and refreshing the book list if an ad is added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var newAdWindow = new NewAdWindow(this.userId);
            bool? result = newAdWindow.ShowDialog();

            if (result == true)
            {
                var books = await this.bookService.GetAllBooksAsync();
                this.AllBooks = new ObservableCollection<BookDto>(books);
                this.FilteredBooks = new ObservableCollection<BookDto>(books);
                this.BooksItemsControl.ItemsSource = this.FilteredBooks;
                this.UpdateResultsCount();
            }
        }

        /// <summary>
        /// Handles the click event on a book card, opening the ad view window for the selected book.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void BookCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int bookId && bookId > 0)
            {
                var viewWindow = new ViewAdWindow(bookId, this.userId);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Не вдалося визначити книгу для перегляду.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateResultsCount()
        {
            int count = this.FilteredBooks?.Count ?? 0;
            string booksWord = count switch
            {
                1 => "книга",
                >= 2 and <= 4 => "книги",
                _ => "книг"
            };
            this.ResultsCountText.Text = $"Знайдено: {count} {booksWord}";
        }

        private void ContextMenu_View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var viewWindow = new ViewAdWindow(book.Id, this.userId);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
            }
        }

        private void ContextMenu_Report_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var reportWindow = new ReportAdWindow(book.Id, this.userId);
                reportWindow.Owner = this;
                reportWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Scrolls the BooksScrollViewer to the bottom.
        /// </summary>
        public void ScrollToBottom()
        {
            // Якщо є ScrollViewer — просто прокручуємо його вниз
            this.BooksScrollViewer?.UpdateLayout();
            this.BooksScrollViewer?.ScrollToEnd();
        }
    }
}