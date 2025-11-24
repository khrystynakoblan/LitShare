using LitShare.BLL.DTOs;
using LitShare.BLL.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ComplaintsPage : Window
    {
        public ObservableCollection<ComplaintDto> Complaints { get; set; }

        public ComplaintsPage()
        {
            InitializeComponent();
            LoadComplaints();
            ComplaintsGrid.ItemsSource = Complaints;
        }

        private void LoadComplaints()
        {
            var service = new ComplaintsService();
            var complaintsList = service.GetAllComplaints();
            Complaints = new ObservableCollection<ComplaintDto>(complaintsList);
        }

        private void ComplaintsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ComplaintsGrid.SelectedItem is ComplaintDto selectedComplaint)
            {
                var reviewWindow = new ComplaintReviewWindow(selectedComplaint.Id);
                reviewWindow.ShowDialog();

                LoadComplaints();
                ComplaintsGrid.ItemsSource = Complaints;
            }
        }

    }
}
