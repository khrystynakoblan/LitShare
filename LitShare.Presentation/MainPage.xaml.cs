using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LitShare.BLL.DTOs;
using LitShare.BLL.Services;

namespace LitShare.Presentation
{
    public partial class MainPage : Window
    {
        public ObservableCollection<BookDto> AllBooks { get; set; } = new();
        public ObservableCollection<BookDto> FilteredBooks { get; set; } = new();

        private readonly BookService _bookService = new();
        private bool isSearchPlaceholder = true;
        private readonly List<CheckBox> genreCheckBoxes = new();

        private readonly int _userId;

        public MainPage(int userId)
        {
            InitializeComponent();
            _userId = userId; // зберігаємо ID користувача
            Loaded += MainPage_Loaded;
        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ResultsCountText.Text = "Завантаження даних...";

                var booksTask = _bookService.GetAllBooksAsync();
                var genresTask = _bookService.GetGenresAsync();

                await Task.WhenAll(booksTask, genresTask);

                AllBooks = new ObservableCollection<BookDto>(booksTask.Result);
                FilteredBooks = new ObservableCollection<BookDto>(booksTask.Result);
                BooksItemsControl.ItemsSource = FilteredBooks;

                var genres = genresTask.Result;
                GenresPanel.Children.Clear();
                genreCheckBoxes.Clear();
                foreach (var genre in genres)
                {
                    var checkBox = new CheckBox { Content = genre, Margin = new Thickness(0, 3, 0, 3) };
                    checkBox.Checked += FilterChanged;
                    checkBox.Unchecked += FilterChanged;
                    GenresPanel.Children.Add(checkBox);
                    genreCheckBoxes.Add(checkBox);
                }

                UpdateResultsCount();
                SetupSearchPlaceholder();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації: {ex.Message}");
            }
        }


        private void FilterChanged(object sender, RoutedEventArgs e) => ApplyFilters();

        private List<string> GetSelectedGenres() =>
            genreCheckBoxes
                .Where(cb => cb.IsChecked == true && cb.Content != null)
                .Select(cb => cb.Content!.ToString()!)
                .ToList();

        private void ApplyFilters()
        {
            if (AllBooks == null) return;

            string? location = LocationTextBox?.Text?.Trim();
            string? search = isSearchPlaceholder ? null : SearchTextBox?.Text?.Trim();
            string? dealType = ExchangeRadio?.IsChecked == true ? "Обмін" :
                              FreeRadio?.IsChecked == true ? "Безкоштовно" : null;
            var selectedGenres = GetSelectedGenres();

            var filtered = _bookService.GetFilteredBooks(AllBooks.ToList(), search, location, dealType, selectedGenres);

            FilteredBooks.Clear();
            foreach (var book in filtered)
                FilteredBooks.Add(book);

            UpdateResultsCount();
        }

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
            var profileWindow = new ProfileWindow(_userId);
            profileWindow.ShowDialog();
        }

        private async void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var newAdWindow = new NewAdWindow(_userId);
            bool? result = newAdWindow.ShowDialog();

            if (result == true)
            {
                // Оновити список книг після додавання
                var books = await _bookService.GetAllBooksAsync();
                AllBooks = new ObservableCollection<BookDto>(books);
                FilteredBooks = new ObservableCollection<BookDto>(books);
                BooksItemsControl.ItemsSource = FilteredBooks;
                UpdateResultsCount();
            }
        }
        private void BookCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int bookId && bookId > 0)
            {
                var viewWindow = new ViewAdWindow(bookId, _userId);
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
