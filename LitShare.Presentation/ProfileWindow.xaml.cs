using System.Windows;

namespace LitShare.Presentation
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
        }

        // --- ЗАГЛУШКИ ДЛЯ КНОПОК ---

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            // ЗАГЛУШКА: Відкриття вікна додавання книги
            MessageBox.Show("Тут відкриється вікно 'Додати книгу'");

            // TODO: Замініть MessageBox на реальне відкриття вікна, коли воно буде готове
            // AddBookWindow addBookWin = new AddBookWindow();
            // addBookWin.Show();
        }

        private void MyBooksButton_Click(object sender, RoutedEventArgs e)
        {
            // ЗАГЛУШКА: Відкриття вікна "Мої книги"
            MessageBox.Show("Тут відкриється вікно 'Мої книги'");

            // TODO: Замініть MessageBox на реальне відкриття вікна
            // MyBooksWindow myBooksWin = new MyBooksWindow();
            // myBooksWin.Show();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // ЗАГЛУШКА: Відкриття вікна редагування профілю
            MessageBox.Show("Тут відкриється вікно 'Редагувати профіль', де будуть кнопки 'Видалити' та 'Вийти'");

            // TODO: Замініть MessageBox на реальне відкриття вікна
            // EditProfileWindow editProfileWin = new EditProfileWindow();
            // editProfileWin.Show();
        }

    }
}