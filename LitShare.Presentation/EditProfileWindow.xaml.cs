using System.Windows;

namespace LitShare.Presentation
{
    public partial class EditProfileWindow : Window
    {
        public EditProfileWindow()
        {
            InitializeComponent();
        }

        // Кнопка "LitShare"
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перехід на головну сторінку (заглушка).", "LitShare");
        }

        // Кнопка "Мій профіль"
        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перехід до Мого профілю (заглушка).", "LitShare");
        }

        // Кнопка "Зберегти зміни"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Зміни збережено (приклад обробки).", "Редагування профілю");
        }

        // Кнопка "Скасувати"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редагування скасовано.", "Редагування профілю");
        }

        // Кнопка "Видалити профіль"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Профіль видалено (демо).", "Редагування профілю");
        }
    }
}