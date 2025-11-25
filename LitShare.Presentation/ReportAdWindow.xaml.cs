// <copyright file="ReportAdWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using LitShare.BLL.Services;
    using LitShare.Presentation;

    /// <summary>
    /// Interaction logic for ReportAdWindow.xaml.
    /// This window allows a user to report a specific advertisement (post) for various reasons.
    /// </summary>
    public partial class ReportAdWindow : Window
    {
        private readonly int adId;
        private readonly ComplaintsService complaintService = new ComplaintsService();
        private readonly int currentUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAdWindow"/> class.
        /// </summary>
        /// <param name="adId">The ID of the advertisement being reported.</param>
        /// <param name="userId">The ID of the user submitting the complaint.</param>
        public ReportAdWindow(int adId, int userId)
        {
            this.InitializeComponent();
            this.adId = adId;
            this.currentUserId = userId;

            // Subscribe to the Loaded event to set up text change listener after components are initialized.
            this.Loaded += (s, e) =>
            {
                // Listener to hide/show the placeholder text in the details box
                this.DetailsTextBox.TextChanged += (s2, e2) =>
                {
                    this.PlaceholderText.Visibility = string.IsNullOrWhiteSpace(this.DetailsTextBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                };
            };
        }

        /// <summary>
        /// Handles the click event for the Submit Button, validates the selection,
        /// constructs the complaint message, and sends it to the service.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string? selectedReason = null;

            // Determine the selected reason from the Radio Buttons, using safe access (?. and ??)
            if (this.FalseInfoRadio.IsChecked == true)
            {
                selectedReason = this.FalseInfoRadio.Content?.ToString() ?? string.Empty;
            }
            else if (this.SpamRadio.IsChecked == true)
            {
                selectedReason = this.SpamRadio.Content?.ToString() ?? string.Empty;
            }
            else if (this.ExchangeRadio.IsChecked == true)
            {
                selectedReason = this.ExchangeRadio.Content?.ToString() ?? string.Empty;
            }
            else if (this.OtherRadio.IsChecked == true)
            {
                selectedReason = this.OtherRadio.Content?.ToString() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(selectedReason))
            {
                this.ShowStatus("Будь ласка, оберіть причину скарги.", Brushes.OrangeRed);
                return;
            }

            string details = this.DetailsTextBox.Text.Trim();
            string fullText = selectedReason;

            // Append details if provided
            if (!string.IsNullOrEmpty(details))
            {
                fullText += ": " + details;
            }

            try
            {
                // Submit the complaint via the business logic service
                this.complaintService.AddComplaint(fullText, this.adId, this.currentUserId);

                this.ShowStatus("Скаргу надіслано!", Brushes.Green);

                // Reset form fields after successful submission
                this.FalseInfoRadio.IsChecked = false;
                this.SpamRadio.IsChecked = false;
                this.ExchangeRadio.IsChecked = false;
                this.OtherRadio.IsChecked = false;
                this.DetailsTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Display error status
                this.ShowStatus($"Помилка: {ex.InnerException?.Message ?? ex.Message}", Brushes.Red);
            }
        }

        /// <summary>
        /// Updates the status message text and color on the UI.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color brush for the text.</param>
        private void ShowStatus(string message, Brush color)
        {
            this.StatusMessage.Text = message;
            this.StatusMessage.Foreground = color;
        }

        /// <summary>
        /// Handles the click event for the My Profile Button, opening the profile window.
        /// Note: The original code uses ShowDialog, which opens the window modally.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming ProfileWindow exists and takes userId
            var profileWindow = new ProfileWindow(this.currentUserId);
            profileWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the click event for the Home Button, opening the main page
        /// and closing the current window.
        /// Note: The original code uses ShowDialog for the main page before closing, which might be unusual.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming MainPage exists and takes userId
            var mainPage = new MainPage(this.currentUserId);
            mainPage.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the Back Button, closing the current window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}