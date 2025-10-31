using System.Windows;

namespace LitShare.Presentation
{
    public partial class ProfileViewWindow : Window
    {
        public ProfileViewWindow()
        {
            InitializeComponent();
        }

        
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перехід на головну сторінку (ще не реалізовано).");
        }

        
        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Перехід у мій профіль (редагування).");
        }

        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Повернення назад на попередню сторінку.");
        }
    }
}