// -----------------------------------------------------------------------
// <copyright file="MyBook.xaml.cs" company="LitShare">
// Copyright (c) 2025 LitShare. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using LitShare; // Необхідно для доступу до MainPage та EditAdWindow, якщо вони в namespace LitShare
    using LitShare.DAL;
    using LitShare.DAL.Models;

    /// <summary>
    /// Логіка взаємодії для вікна "Мої книги".
    /// </summary>
    public partial class MyBook : Window
    {
        private readonly LitShareDbContext _context;
        private readonly int _userId;
        private List<BookItem> _allBooks = new List<BookItem>(); // Ініціалізація для уникнення null

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBook"/> class.
        /// </summary>
        /// <param name="userId">Ідентифікатор поточного користувача.</param>
        public MyBook(int userId)
        {
            InitializeComponent();
            _context = new LitShareDbContext();
            _userId = userId;

            LoadBooks();
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void LoadBooks()
        {
            try
            {
                _allBooks = _context.Posts
                    .Where(p => p.UserId == _userId)
                    .Join(
                        _context.Users,
                        p => p.UserId,
                        u => u.Id,
                        (p, u) => new BookItem
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Author = p.Author,
                            City = u.City,
                            PhotoUrl = p.PhotoUrl
                        })
                    .ToList();

                DisplayBooks(_allBooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні книг: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                DisplayBooks(_allBooks);
            }
            else
            {
                var filteredBooks = _allBooks.Where(b =>
                    (b.Title?.ToLower().Contains(searchText) == true) ||
                    (b.Author?.ToLower().Contains(searchText) == true) ||
                    (b.City?.ToLower().Contains(searchText) == true)
                ).ToList();

                DisplayBooks(filteredBooks);
            }
        }

        private void DisplayBooks(List<BookItem> books)
        {
            BooksItemsControl.ItemsSource = books;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainPage(_userId);
            mainWindow.Show();
            Close();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BookItem selectedBook)
            {
                var editWindow = new EditAdWindow(selectedBook.Id, _userId);
                editWindow.Owner = this;

                editWindow.ShowDialog();

                LoadBooks();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Внутрішній клас для відображення спрощеної інформації про книгу.
        /// </summary>
        private class BookItem
        {
            /// <summary>
            /// Gets or sets the unique identifier of the book.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the title of the book.
            /// </summary>
            public string Title { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the author of the book.
            /// </summary>
            public string Author { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the city where the book is located.
            /// </summary>
            public string City { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the URL of the book's photo.
            /// </summary>
            public string? PhotoUrl { get; set; }
        }
    }
}