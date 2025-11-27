// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System; // Required for Exception
    using System.Windows;
    using LitShare.BLL.Logging;
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// This is the main window of the application, responsible for displaying and managing users.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The user service instance used for business logic operations.
        /// </summary>
        private readonly UserService userService = new UserService();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            AppLogger.Info("MainWindow створено");
        }

        /// <summary>
        /// Handles the Loaded event for the Window.
        /// Populates the users grid with a list of all users from the service layer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppLogger.Info("Спроба завантаження списку користувачів у MainWindow");
            try
            {
                var allUsers = this.userService.GetAllUsers();
                this.usersGrid.ItemsSource = allUsers;

                AppLogger.Info($"Успішно завантажено користувачів: {allUsers?.Count()}");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"ПОМИЛКА при завантаженні користувачів у MainWindow: {ex.Message}");

                MessageBox.Show(
                    $"LOADING ERROR: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}