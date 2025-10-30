using System.Windows;

namespace LitShare.Presentation
{
    public partial class ProfileViewWindow : Window
    {
        public ProfileViewWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}