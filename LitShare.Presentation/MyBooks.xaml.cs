using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LitShare
{
    public partial class MainWindow : Window
    {
        private List<Book> books = new List<Book>();

        public MainWindow()
        {
            InitializeComponent();
            LoadBooks();
            DisplayBooks(books);
        }

        public class Book
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string City { get; set; }
        }

        private void LoadBooks()
        {
            books = new List<Book>
            {
                new Book { Title = "Кобзар", Author = "Тарас Шевченко", City = "Київ" },
                new Book { Title = "Місто", Author = "Валер’ян Підмогильний", City = "Харків" },
                new Book { Title = "Тигролови", Author = "Іван Багряний", City = "Львів" },
                new Book { Title = "Захар Беркут", Author = "Іван Франко", City = "Дрогобич" },
                new Book { Title = "Кобзар", Author = "Тарас Шевченко", City = "Київ" },
                new Book { Title = "Місто", Author = "Валер’ян Підмогильний", City = "Харків" },
                new Book { Title = "Тигролови", Author = "Іван Багряний", City = "Львів" },
                new Book { Title = "Захар Беркут", Author = "Іван Франко", City = "Дрогобич" },
                new Book { Title = "Кобзар", Author = "Тарас Шевченко", City = "Київ" },
                new Book { Title = "Місто", Author = "Валер’ян Підмогильний", City = "Харків" },
                new Book { Title = "Тигролови", Author = "Іван Багряний", City = "Львів" },
                new Book { Title = "Захар Беркут", Author = "Іван Франко", City = "Дрогобич" },
                new Book { Title = "Кобзар", Author = "Тарас Шевченко", City = "Київ" },
                new Book { Title = "Місто", Author = "Валер’ян Підмогильний", City = "Харків" },
                new Book { Title = "Тигролови", Author = "Іван Багряний", City = "Львів" },
                new Book { Title = "Захар Беркут", Author = "Іван Франко", City = "Дрогобич" }
            };
        }

        private void DisplayBooks(List<Book> bookList)
        {
            var uniformGrid = FindUniformGrid();

            if (uniformGrid == null)
                return;

            uniformGrid.Children.Clear();

            foreach (var book in bookList)
            {
                Border card = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Background = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(10)
                };

                StackPanel stack = new StackPanel { Margin = new Thickness(10) };

                Border cover = new Border
                {
                    Width = 120,
                    Height = 160,
                    Background = System.Windows.Media.Brushes.LightGray,
                    CornerRadius = new CornerRadius(4),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                cover.Child = new TextBlock
                {
                    Text = "🖼️",
                    FontSize = 32,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                StackPanel info = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                info.Children.Add(new TextBlock { Text = book.Title, FontWeight = FontWeights.SemiBold, HorizontalAlignment = HorizontalAlignment.Center });
                info.Children.Add(new TextBlock { Text = book.Author, FontSize = 12, Foreground = System.Windows.Media.Brushes.Gray, HorizontalAlignment = HorizontalAlignment.Center });
                info.Children.Add(new TextBlock { Text = book.City, FontSize = 12, Foreground = System.Windows.Media.Brushes.Gray, HorizontalAlignment = HorizontalAlignment.Center });

                Button editButton = new Button
                {
                    Content = "Редагувати",
                    Height = 30,
                    Background = System.Windows.Media.Brushes.Black,
                    Foreground = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 10, 0, 0),
                    Tag = book // збереження посилання на книгу
                };
                editButton.Click += EditButton_Click;

                stack.Children.Add(cover);
                stack.Children.Add(info);
                stack.Children.Add(editButton);
                card.Child = stack;

                uniformGrid.Children.Add(card);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Book book)
            {
                MessageBox.Show($"Редагування книги:\n\nНазва: {book.Title}\nАвтор: {book.Author}\nМісто: {book.City}",
                    "Редагування", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private UniformGrid FindUniformGrid()
        {
            foreach (var child in LogicalTreeHelper.GetChildren(this))
            {
                if (child is ScrollViewer scroll)
                {
                    if (scroll.Content is UniformGrid grid)
                        return grid;
                }
                else if (child is DockPanel dockPanel)
                {
                    foreach (var subChild in LogicalTreeHelper.GetChildren(dockPanel))
                    {
                        if (subChild is ScrollViewer scroll2 && scroll2.Content is UniformGrid grid2)
                            return grid2;
                    }
                }
            }
            return null;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchTextBox.Text.ToLower();
            var filtered = books.FindAll(b =>
                b.Title.ToLower().Contains(query) ||
                b.Author.ToLower().Contains(query) ||
                b.City.ToLower().Contains(query));
            DisplayBooks(filtered);
        }
    }
}
