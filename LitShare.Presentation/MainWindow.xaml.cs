using System;
using System.Windows;
using LitShare.BLL.Services; // <-- Ось де потрібні 'using'

namespace LitShare.Presentation
{
    public partial class MainWindow : Window
    {
        private readonly UserService _userService = new UserService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var allUsers = _userService.GetAllUsers();
                usersGrid.ItemsSource = allUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ПОМИЛКА ЗАВАНТАЖЕННЯ: {ex.Message}",
                                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}