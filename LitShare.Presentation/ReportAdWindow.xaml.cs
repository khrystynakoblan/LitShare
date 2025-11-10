using LitShare.BLL.Services;
using System;
using System.Windows;
using System.Windows.Media;

namespace LitShare.Presentation
{
    public partial class ReportAdWindow : Window
    {
        private readonly int _adId;
        private readonly ComplaintsService _complaintService = new ComplaintsService();
        private readonly int _currentUserId;

        public ReportAdWindow(int adId, int userId)
        {
            InitializeComponent();
            _adId = adId;
            _currentUserId = userId;

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
                ShowStatus("Будь ласка, оберіть причину скарги.", Brushes.OrangeRed);
                return;
            }

            string details = DetailsTextBox.Text.Trim();
            string fullText = selectedReason;
            if (!string.IsNullOrEmpty(details))
                fullText += ": " + details;

            try
            {
                _complaintService.AddComplaint(fullText, _adId, _currentUserId);

                ShowStatus("Скаргу надіслано!", Brushes.Green);

                FalseInfoRadio.IsChecked = false;
                SpamRadio.IsChecked = false;
                ExchangeRadio.IsChecked = false;
                OtherRadio.IsChecked = false;
                DetailsTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ShowStatus($"Помилка: {ex.InnerException?.Message ?? ex.Message}", Brushes.Red);
            }
        }

        private void ShowStatus(string message, Brush color)
        {
            StatusMessage.Text = message;
            StatusMessage.Foreground = color;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
