using LitShare.BLL.Services;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace LitShare.Presentation
{
    public partial class ComplaintReviewWindow : Window
    {
        private readonly ComplaintsService _complaintsService = new ComplaintsService();
        private readonly UserService _userService = new UserService();

        private int _currentComplaintId;

        public ComplaintReviewWindow(int complaintId)
        {
            InitializeComponent();
            _currentComplaintId = complaintId;
            _ = LoadComplaintDataAsync(_currentComplaintId);
        }

        private async Task LoadComplaintDataAsync(int complaintId)
        {
            try
            {
                var complaint = _complaintsService.GetComplaintWithDetails(complaintId);

                if (complaint != null && complaint.Post != null)
                {
                    txtComplaintReason.Text = complaint.text;
                    txtPostTitle.Text = complaint.Post.title;
                    txtPostDescription.Text = complaint.Post.description;

                    var author = _userService.GetUserById(complaint.Post.user_id);
                    txtPostAuthor.Text = author?.name ?? "Невідомий автор";

                    LoadImage(complaint.Post.photo_url);
                }
                else
                {
                    MessageBox.Show($"Не вдалося завантажити скаргу з ID {complaintId}.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критична помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LoadImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                postImage.Source = null;
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                postImage.Source = bitmap;
            }
            catch
            {
                postImage.Source = null;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _complaintsService.ApproveComplaint(_currentComplaintId);

                await ShowToast("✔ Скаргу підтверджено");

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _complaintsService.DeleteComplaint(_currentComplaintId);

                await ShowToast("✖ Скаргу відхилено");

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async Task ShowToast(string text)
        {
            ToastText.Text = text;
            Toast.Visibility = Visibility.Visible;

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
            Toast.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(1500);

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
            fadeOut.Completed += (s, e) => Toast.Visibility = Visibility.Collapsed;

            Toast.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}