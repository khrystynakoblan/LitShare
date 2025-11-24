// <copyright file="ComplaintsPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.Presentation
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;

    public partial class ComplaintsPage : Window
    {
        public ObservableCollection<ComplaintDto> Complaints { get; set; }

        public ComplaintsPage()
        {
            this.InitializeComponent();
            this.LoadComplaints();
            this.ComplaintsGrid.ItemsSource = this.Complaints;
        }

        private void LoadComplaints()
        {
            var service = new ComplaintsService();
            var complaintsList = service.GetAllComplaints();
            this.Complaints = new ObservableCollection<ComplaintDto>(complaintsList);
        }

        private void ComplaintsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.ComplaintsGrid.SelectedItem is ComplaintDto selectedComplaint)
            {
                var reviewWindow = new ComplaintReviewWindow(selectedComplaint.Id);
                reviewWindow.ShowDialog();

                this.LoadComplaints();
                this.ComplaintsGrid.ItemsSource = this.Complaints;
            }
        }

    }
}
