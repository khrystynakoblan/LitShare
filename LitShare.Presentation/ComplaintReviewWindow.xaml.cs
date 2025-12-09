// <copyright file="ComplaintReviewWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using LitShare.BLL.Logging;
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for ComplaintReviewWindow.xaml.
    /// This window is used by administrators or moderators to review a specific user complaint against an advertisement.
    /// </summary>
    public partial class ComplaintReviewWindow : Window
    {
        private readonly ComplaintsService complaintsService = new ComplaintsService();
        private readonly UserService userService = new UserService();

        private int currentComplaintId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintReviewWindow"/> class.
        /// Asynchronously loads the details of the complaint and the associated post.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public ComplaintReviewWindow(int complaintId)
        {
            this.InitializeComponent();
            this.currentComplaintId = complaintId;

            AppLogger.Info($"Відкрито ComplaintReviewWindow для скарги Id={complaintId}");

            _ = this.LoadComplaintDataAsync(this.currentComplaintId);
        }

        /// <summary>
        /// Asynchronously loads the details of the complaint and the associated post.
        /// </summary>
        /// <param name="complaintId">The ID of the complaint to load.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task LoadComplaintDataAsync(int complaintId)
        {
            try
            {
                var complaint = this.complaintsService.GetComplaintWithDetails(complaintId);

                if (complaint != null && complaint.Post != null)
                {
                    this.txtComplaintReason.Text = complaint.Text;
                    this.txtPostTitle.Text = complaint.Post.Title;
                    this.txtPostDescription.Text = complaint.Post.Description;

                    var author = this.userService.GetUserById(complaint.Post.UserId);
                    this.txtPostAuthor.Text = author?.Name ?? "Невідомий автор";

                    this.LoadImage(complaint.Post.PhotoUrl);

                    AppLogger.Info($"Скарга Id={complaintId} успішно завантажена");
                }
                else
                {
                    AppLogger.Warn($"Не вдалося завантажити скаргу з ID {complaintId}");

                    MessageBox.Show($"Не вдалося завантажити скаргу з ID {complaintId}.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationManager.GoBack();

                }
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Критична помилка при завантаженні скарги Id={complaintId}", ex);
                MessageBox.Show($"Критична помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationManager.GoBack();

            }

            return Task.CompletedTask;
        }

        private void LoadImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                this.postImage.Source = null;
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                this.postImage.Source = bitmap;

                AppLogger.Info($"Завантажено фото скарги: {url}");
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"Не вдалося завантажити фото скарги: {url}. {ex.Message}");
                this.postImage.Source = null;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Вікно ComplaintReviewWindow закрито вручну");
            NavigationManager.GoBack();

        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.complaintsService.ApproveComplaint(this.currentComplaintId);
                AppLogger.Info($"Оголошення для скарги Id={this.currentComplaintId} видалено");

                await this.ShowToast("✔ Оголошення видалено");

                NavigationManager.GoBack();

            }
            catch (Exception ex)
            {
                AppLogger.Error($"Помилка при видаленні оголошення для скарги Id={this.currentComplaintId}", ex);
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.complaintsService.DeleteComplaint(this.currentComplaintId);
                AppLogger.Info($"Скаргу Id={this.currentComplaintId} видалено");

                await this.ShowToast("✖ Скаргу видалено");

                NavigationManager.GoBack();
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Помилка при видаленні скарги Id={this.currentComplaintId}", ex);
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async Task ShowToast(string text)
        {
            this.ToastText.Text = text;
            this.Toast.Visibility = Visibility.Visible;

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
            this.Toast.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(1500);

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
            fadeOut.Completed += (s, e) => this.Toast.Visibility = Visibility.Collapsed;

            this.Toast.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info("Повернення з ComplaintReviewWindow");
            NavigationManager.GoBack();
        }
    }
}