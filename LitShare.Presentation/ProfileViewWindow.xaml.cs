//-----------------------------------------------------------------------
// <copyright file="ProfileViewWindow.xaml.cs" company="LitShare.Presentation">
//     Copyright (c) LitShare.Presentation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace LitShare.Presentation
{
    // SA1200: Всі using перенесені всередину namespace
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;

    /// <summary>
    /// Вікно для перегляду профілю іншого користувача та його оголошень.
    /// </summary>
    public partial class ProfileViewWindow : Window
    {
        private readonly UserService userService = new UserService();
        private readonly BookService bookService = new BookService();
        private readonly int currentUserId;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="ProfileViewWindow"/>.
        /// </summary>
        /// <param name="userBookId">Ідентифікатор користувача, чий профіль потрібно переглянути.</param>
        /// <param name="userId">Ідентифікатор поточного (залогованого) користувача.</param>
        public ProfileViewWindow(int userBookId, int userId)
        {
            this.InitializeComponent();
            this.currentUserId = userId;

            // CS4014: Явно ігноруємо результат асинхронного методу в конструкторі
            _ = this.LoadUserProfileAsync(userBookId);
        }

        /// <summary>
        /// Завантажує дані профілю користувача та його книги асинхронно.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача для завантаження.</param>
        private async Task LoadUserProfileAsync(int userId)
        {
            try
            {
                var user = this.userService.GetUserProfileById(userId);

                if (user != null)
                {
                    this.txtName.Text = user.name;
                    this.txtRegion.Text = user.region;
                    this.txtDistrict.Text = user.district;
                    this.txtCity.Text = user.city;
                    this.txtPhone.Text = user.phone ?? "—";
                    this.txtAbout.Text = user.about ?? "Користувач ще не заповнив інформацію про себе.";

                    var books = await this.bookService.GetBooksByUserIdAsync(userId);
                    this.BooksList.ItemsSource = books;
                }
                else
                {
                    MessageBox.Show("Користувача не знайдено!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                // Логіка обробки помилок завантаження профілю
                MessageBox.Show($"Помилка завантаження профілю: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обробник натискання кнопки "Головна". Переходить на головну сторінку.
        /// </summary>
        /// <param name="sender">Об'єкт-відправник.</param>
        /// <param name="e">Аргументи події.</param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainPage(this.currentUserId);
            mainPage.Show();
            this.Close();
        }

        /// <summary>
        /// Обробник натискання кнопки "Мій профіль". Відкриває вікно редагування профілю.
        /// </summary>
        /// <param name="sender">Об'єкт-відправник.</param>
        /// <param name="e">Аргументи події.</param>
        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(this.currentUserId);
            profileWindow.ShowDialog();
        }

        /// <summary>
        /// Обробник натискання кнопки "Назад". Закриває поточне вікно.
        /// </summary>
        /// <param name="sender">Об'єкт-відправник.</param>
        /// <param name="e">Аргументи події.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Обробник контекстного меню: перегляд оголошення.
        /// </summary>
        /// <param name="sender">Об'єкт-відправник.</param>
        /// <param name="e">Аргументи події.</param>
        private void ContextMenu_View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var viewWindow = new ViewAdWindow(book.Id, this.currentUserId);
                viewWindow.Owner = this;
                viewWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Обробник контекстного меню: поскаржитися на оголошення.
        /// </summary>
        /// <param name="sender">Об'єкт-відправник.</param>
        /// <param name="e">Аргументи події.</param>
        private void ContextMenu_Report_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.DataContext is BookDto book)
            {
                var reportWindow = new ReportAdWindow(book.Id, this.currentUserId);
                reportWindow.Owner = this;
                reportWindow.ShowDialog();
            }
        }
    }
}