using System.Windows; // Потрібно для Window, MessageBox, RoutedEventArgs

namespace LitShare.Presentation
{
    /// <summary>
    /// Логіка взаємодії для ComplaintReviewWindow.xaml
    /// </summary>
    public partial class ComplaintReviewWindow : Window
    {
        public ComplaintReviewWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Додати тут логіку виходу(мало б закрити скаргу і перейти до сторінки зі спсиком скарг)

            // Просто закриваємо це вікно
            this.Close();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Додати сюди логіку для підтвердження скарги
            // оголошення має видалитися з бд. перейти до сторінки зі спсиком скарг

            MessageBox.Show("Скаргу було підтверджено.");

            this.Close();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Додайте сюди логіку для відхилення скарги
            // скарга має видалитися. перейти до сторінки зі спсиком скарг

            MessageBox.Show("Скаргу було відхилено.");

            // Закриваємо вікно після дії
            this.Close();
        }
    }
}