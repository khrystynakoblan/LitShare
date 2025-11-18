// -----------------------------------------------------------------------
// <copyright file="ComplaintReviewWindow.xaml.cs" company="LitShare">
// Copyright (c) 2025 LitShare. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.Presentation
{
    using System;
    using System.Windows;
    using LitShare.BLL.Services;

    /// <summary>
    /// Логіка взаємодії для вікна перегляду деталей скарги.
    /// </summary>
    public partial class ComplaintReviewWindow : Window
    {
        private readonly ComplaintsService _complaintsService = new ComplaintsService();
        private readonly UserService _userService = new UserService();
        private readonly int _currentComplaintId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintReviewWindow"/> class.
        /// </summary>
        /// <param name="complaintId">Ідентифікатор скарги для перегляду.</param>
        public ComplaintReviewWindow(int complaintId)
        {
            InitializeComponent();
            _currentComplaintId = complaintId;
            LoadComplaintData(_currentComplaintId);
        }

        private void LoadComplaintData(int complaintId)
        {
            try
            {
                var complaint = _complaintsService.GetComplaintWithDetails(complaintId);

                if (complaint != null && complaint.Post != null)
                {
                    txtComplaintReason.Text = complaint.Text;
                    txtPostTitle.Text = complaint.Post.Title;
                    txtPostDescription.Text = complaint.Post.Description;

                    var author = _userService.GetUserById(complaint.Post.UserId);
                    txtPostAuthor.Text = author?.Name ?? "Невідомий автор";
                }
                else
                {
                    MessageBox.Show($"Не вдалося завантажити скаргу з ID {complaintId}.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критична помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Скаргу підтверджено. (Тут має бути логіка покарання)", "Інфо", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Скаргу відхилено.", "Інфо", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}