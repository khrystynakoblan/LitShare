using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LitShare.DAL;
using LitShare.DAL.Models;

namespace LitShare.Presentation
{
    public partial class MyBook : Window
    {
        private readonly LitShareDbContext _context;
        private List<BookItem> _allBooks;

        public MyBook()
        {
            InitializeComponent();
            _context = new LitShareDbContext();
            LoadBooks();
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void LoadBooks()
        {
            try
            {
                _allBooks = _context.posts
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
            BooksGrid.Children.Clear();

            if (books.Count == 0)
            {
                var noResultsText = new TextBlock
                {
                    Text = "Книги не знайдено",
                    FontSize = 16,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                BooksGrid.Children.Add(noResultsText);
                return;
            }

            foreach (var book in books)
            {
                var card = CreateBookCard(book.Title, book.Author, book.City);
                BooksGrid.Children.Add(card);
            }
        }

        private Border CreateBookCard(string title, string author, string city)
        {
            var border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Background = System.Windows.Media.Brushes.White,
                Height = 285,
                Margin = new Thickness(10)
            };

            var stack = new StackPanel { Margin = new Thickness(10) };

            var cover = new Border
            {
                Width = 120,
                Height = 160,
                Background = System.Windows.Media.Brushes.LightGray,
                CornerRadius = new CornerRadius(4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            cover.Child = new TextBlock
            {
                Text = "🖼️",
                FontSize = 32,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var info = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            info.Children.Add(new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 140
            });

            info.Children.Add(new TextBlock
            {
                Text = author,
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 140
            });

            info.Children.Add(new TextBlock
            {
                Text = city,
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var editButton = new Button
            {
                Content = "Редагувати",
                Height = 30,
                Background = System.Windows.Media.Brushes.Black,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            stack.Children.Add(cover);
            stack.Children.Add(info);
            stack.Children.Add(editButton);

            border.Child = stack;

            return border;
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