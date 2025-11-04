using LitShare.BLL.Services;
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ReportAdWindow : Window
    {
        private readonly int _adId;
        private readonly ComplaintService _complaintService = new ComplaintService();
        private readonly int _currentUserId = 1; // Тут заміни на ID поточного користувача з авторизації

        public ReportAdWindow(int adId)
        {
            InitializeComponent();
            _adId = adId;

            Loaded += (s, e) =>
            {
                DetailsTextBox.TextChanged += (s2, e2) =>
                {
                    PlaceholderText.Visibility = string.IsNullOrWhiteSpace(DetailsTextBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                };
            };
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedReason = null;

            if (FalseInfoRadio.IsChecked == true)
                selectedReason = FalseInfoRadio.Content.ToString();
            else if (SpamRadio.IsChecked == true)
                selectedReason = SpamRadio.Content.ToString();
            else if (ExchangeRadio.IsChecked == true)
                selectedReason = ExchangeRadio.Content.ToString();
            else if (OtherRadio.IsChecked == true)
                selectedReason = OtherRadio.Content.ToString();

            if (string.IsNullOrEmpty(selectedReason))
            {
                MessageBox.Show("Будь ласка, оберіть причину скарги.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string details = DetailsTextBox.Text.Trim();
            string fullText = selectedReason;
            if (!string.IsNullOrEmpty(details))
                fullText += ": " + details;

            try
            {
                _complaintService.AddComplaint(_adId, _currentUserId, fullText);
                MessageBox.Show("Скаргу надіслано. Дякуємо!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Сталася помилка при збереженні скарги.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
