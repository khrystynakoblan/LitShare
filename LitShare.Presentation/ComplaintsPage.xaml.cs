// <copyright file="ComplaintsPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Logging;
    using LitShare.BLL.Services;

    /// <summary>
    /// Interaction logic for ComplaintsPage.xaml.
    /// This window displays a list of all complaints and allows for their review.
    /// </summary>
    public partial class ComplaintsPage : Window
    {
        // SA1201: Поля та Конструктори йдуть перед Властивостями.

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintsPage"/> class.
        /// </summary>
        public ComplaintsPage()
        {
            this.InitializeComponent();

            AppLogger.Info("Відкрито ComplaintsPage");

            try
            {
                this.LoadComplaints();
                this.ComplaintsGrid.ItemsSource = this.Complaints;
                AppLogger.Info($"Завантажено {this.Complaints.Count} скарг");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Помилка при завантаженні скарг", ex);
                MessageBox.Show($"Помилка при завантаженні скарг: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets or sets the collection of complaints to be displayed.
        /// Використовується ініціалізація, щоб уникнути попередження CS8618.
        /// </summary>
        public ObservableCollection<ComplaintDto> Complaints { get; set; } = new ObservableCollection<ComplaintDto>();

        /// <summary>
        /// Loads all complaints from the service layer into the <see cref="Complaints"/> collection.
        /// </summary>
        private void LoadComplaints()
        {
            try
            {
                var service = new ComplaintsService();
                var complaintsList = service.GetAllComplaints();

                if (complaintsList != null)
                {
                    this.Complaints = new ObservableCollection<ComplaintDto>(complaintsList);
                    AppLogger.Info($"LoadComplaints: отримано {complaintsList.Count} скарг");
                }
                else
                {
                    AppLogger.Warn("LoadComplaints: сервіс повернув null");
                    this.Complaints = new ObservableCollection<ComplaintDto>();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Помилка при отриманні скарг у LoadComplaints", ex);
                throw;
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event on the ComplaintsGrid.
        /// Opens a new ComplaintReviewWindow for the selected complaint and reloads the list after the review.
        /// </summary>
        /// <param name="sender">The source of the event, which is the ComplaintsGrid.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ComplaintsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ComplaintsGrid.SelectedItem is ComplaintDto selectedComplaint)
            {
                AppLogger.Info($"Відкрито ComplaintReviewWindow для скарги Id={selectedComplaint.Id}");

                try
                {
                    var reviewWindow = new ComplaintReviewWindow(selectedComplaint.Id);
                    reviewWindow.ShowDialog();

                    this.LoadComplaints();
                    this.ComplaintsGrid.ItemsSource = this.Complaints;
                    AppLogger.Info("ComplaintsPage оновлено після перегляду скарги");
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"Помилка при відкритті ComplaintReviewWindow для Id={selectedComplaint.Id}", ex);
                }
            }
        }
    }
}