using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LitShare.DAL;
using LitShare.DAL.Models;

namespace LitShare.Presentation
{
    public partial class MyBook : Window
    {
        private readonly LitShareDbContext _context;
        private List<BookItem> _allBooks;
        private readonly int _userId;

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
                int currentUserId = _userId;

                _allBooks = _context.posts
                    .Where(p => p.user_id == currentUserId)
                    .Join(_context.Users, p => p.user_id, u => u.id, (p, u) => new BookItem
                    {
                        Id = p.id,
                        Title = p.title,
                        Author = p.author,
                        City = u.city,
                        PhotoUrl = p.photo_url
                    })
                    .ToList();

                DisplayBooks(_allBooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні книг: {ex.Message}");
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
                    b.Title.ToLower().Contains(searchText) ||
                    b.Author.ToLower().Contains(searchText) ||
                    b.City.ToLower().Contains(searchText)
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
            MainPage mainWindow = new MainPage(_userId);
            mainWindow.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private class BookItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string City { get; set; }
            public string PhotoUrl { get; set; }
        }
    }
}
