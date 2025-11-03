using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LitShare.BLL.DTOs;
using LitShare.BLL.Services;

namespace LitShare.Presentation
{
    public partial class MainPage : Window
    {
        public ObservableCollection<BookDto> AllBooks { get; set; }
        public ObservableCollection<BookDto> FilteredBooks { get; set; }

        private readonly BookService _bookService;
        private bool isSearchPlaceholder = true;
        private List<CheckBox> genreCheckBoxes = new();

        public MainPage()
        {
            InitializeComponent();
            _bookService = new BookService();

            InitializeBooks();
            FilteredBooks = new ObservableCollection<BookDto>(AllBooks);
            BooksItemsControl.ItemsSource = FilteredBooks;
            UpdateResultsCount();

            LoadGenres();
            SetupSearchPlaceholder();
        }

        // Ініціалізація та завантаження даних
        private void InitializeBooks()
        {
            try
            {
                var groupedBooks = _bookService.GetAllBooks();
                AllBooks = new ObservableCollection<BookDto>(groupedBooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження книг: {ex.Message}");
                AllBooks = new ObservableCollection<BookDto>();
            }
        }

        private void LoadGenres()
        {
            try
            {
                var genres = _bookService.GetGenres();

                GenresPanel.Children.Clear();
                genreCheckBoxes.Clear();

                foreach (var genre in genres)
                {
                    var checkBox = new CheckBox
                    {
                        Content = genre,
                        Margin = new Thickness(0, 3, 0, 3)
                    };

                    checkBox.Checked += FilterChanged;
                    checkBox.Unchecked += FilterChanged;

                    GenresPanel.Children.Add(checkBox);
                    genreCheckBoxes.Add(checkBox);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження жанрів: {ex.Message}");
            }
        }

        // Пошук і фільтрація
        private void FilterChanged(object sender, RoutedEventArgs e) => ApplyFilters();

        private List<string> GetSelectedGenres()
        {
            return genreCheckBoxes
                .Where(cb => cb.IsChecked == true && cb.Content != null)
                .Select(cb => cb.Content!.ToString()!)
                .ToList();
        }

        private void ApplyFilters()
        {
            if (AllBooks == null) return;

            string? location = LocationTextBox?.Text?.Trim();
            string? search = isSearchPlaceholder ? null : SearchTextBox?.Text?.Trim();
            string? dealType = ExchangeRadio?.IsChecked == true ? "Обмін" :
                              FreeRadio?.IsChecked == true ? "Безкоштовно" : null;
            var selectedGenres = GetSelectedGenres();

            var filtered = _bookService.GetFilteredBooks(
                AllBooks.ToList(), search, location, dealType, selectedGenres);

            FilteredBooks.Clear();
            foreach (var book in filtered)
                FilteredBooks.Add(book);

            UpdateResultsCount();
        }


        // Пошук — Placeholder
        private void SetupSearchPlaceholder()
        {
            SearchTextBox.Text = "Пошук...";
            SearchTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            isSearchPlaceholder = true;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isSearchPlaceholder)
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = new SolidColorBrush(Colors.Black);
                isSearchPlaceholder = false;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                SetupSearchPlaceholder();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isSearchPlaceholder)
                ApplyFilters();
        }


        private void MyProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут буде сторінка профілю користувача.");
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут буде сторінка додавання книги.");
        }

        private void BookCard_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут буде сторінка перегляду оголошення.");
        }

        // Лічильник результатів
        private void UpdateResultsCount()
        {
            int count = FilteredBooks?.Count ?? 0;
            string booksWord = count switch
            {
                1 => "книга",
                >= 2 and <= 4 => "книги",
                _ => "книг"
            };
            ResultsCountText.Text = $"Знайдено: {count} {booksWord}";
        }
    }
}