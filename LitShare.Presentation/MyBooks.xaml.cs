// <copyright file="MyBooks.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using LitShare.BLL.Logging;
    using LitShare.DAL;

    /// <summary>
    /// Interaction logic for MyBook.xaml. This window displays all books posted by the current user.
    /// </summary>
    public partial class MyBook : Window
    {
        private readonly LitShareDbContext context;
        private readonly int userId;

        private List<BookItem> allBooks = new List<BookItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBook"/> class.
        /// </summary>
        /// <param name="userId">The ID of the currently logged-in user.</param>
        public MyBook(int userId)
        {
            this.InitializeComponent();
            this.context = new LitShareDbContext();
            this.userId = userId;

            AppLogger.Info($"Відкрито вікно MyBook для користувача ID = {userId}");

            this.LoadBooks();
            this.SearchTextBox.TextChanged += this.SearchTextBox_TextChanged;
        }

        /// <summary>
        /// Loads the list of books posted by the current user from the database.
        /// </summary>
        private void LoadBooks()
        {
            try
            {
                int currentUserId = this.userId;

                this.allBooks = this.context.Posts
                    .Where(p => p.UserId == currentUserId)
                    .Join(this.context.Users, p => p.UserId, u => u.Id, (p, u) => new BookItem
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Author = p.Author,
                        City = u.City,
                        PhotoUrl = p.PhotoUrl,
                    })
                    .ToList();

                AppLogger.Info($"Завантажено {this.allBooks.Count} книг користувача ID = {this.userId}");

                this.DisplayBooks(this.allBooks);
            }
            catch (Exception ex)
            {
                AppLogger.Error($"ПОМИЛКА завантаження книг користувача ID = {this.userId}: {ex}");
                MessageBox.Show($"Помилка при завантаженні книг: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var searchText = this.SearchTextBox.Text.ToLower().Trim();

                if (string.IsNullOrEmpty(searchText))
                {
                    this.DisplayBooks(this.allBooks);
                    return;
                }

                var filteredBooks = this.allBooks
                    .Where(b =>
                        (b.Title ?? string.Empty).ToLower().Contains(searchText) ||
                        (b.Author ?? string.Empty).ToLower().Contains(searchText) ||
                        (b.City ?? string.Empty).ToLower().Contains(searchText))
                    .ToList();

                this.DisplayBooks(filteredBooks);
            }
            catch (Exception ex)
            {
                AppLogger.Error($"ПОМИЛКА під час пошуку книг: {ex}");
            }
        }

        /// <summary>
        /// Sets the ItemsSource for the BooksItemsControl.
        /// </summary>
        /// <param name="books">The list of books to display.</param>
        private void DisplayBooks(List<BookItem> books)
        {
            this.BooksItemsControl.ItemsSource = books;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Перехід на головну сторінку з MyBook. Користувач ID = {this.userId}");
            MainPage mainWindow = new MainPage(this.userId);
            mainWindow.Show();
            this.Close();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BookItem selectedBook)
            {
                AppLogger.Info($"Редагування книги ID = {selectedBook.Id} користувачем ID = {this.userId}");

                var editWindow = new EditAdWindow(selectedBook.Id, this.userId);
                editWindow.Owner = this;
                this.Hide();
                editWindow.ShowDialog();

                this.LoadBooks();
            }
            else
            {
                AppLogger.Warn("Спроба редагування книги без вибраного елемента");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info("Закриття вікна MyBook кнопкою Назад");
            this.Close();
        }

        /// <summary>
        /// Represents a simplified data structure for displaying a book item in the list.
        /// </summary>
        private class BookItem
        {
            /// <summary>
            /// Gets or sets the book ID.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the book title.
            /// </summary>
            public string? Title { get; set; }

            /// <summary>
            /// Gets or sets the book author.
            /// </summary>
            public string? Author { get; set; }

            /// <summary>
            /// Gets or sets the city where the book is located.
            /// </summary>
            public string? City { get; set; }

            /// <summary>
            /// Gets or sets the URL of the book's photo.
            /// </summary>
            public string? PhotoUrl { get; set; }
        }
    }
}