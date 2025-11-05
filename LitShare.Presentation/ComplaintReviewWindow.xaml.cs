using LitShare.BLL.Services;
using LitShare.DAL.Models; 
using System.Threading.Tasks;  
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ComplaintReviewWindow : Window
    {
        private readonly ComplaintsService _complaintsService = new ComplaintsService();
        private readonly UserService _userService = new UserService(); 

        private int _currentComplaintId;

        public ComplaintReviewWindow()
        {
            InitializeComponent();

            int testComplaintId = 1; 

            _currentComplaintId = testComplaintId; 

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



        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ТЕСТ: Скаргу 'Підтверджено' (нічого не зроблено)");
            this.Close();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ТЕСТ: Скаргу 'Відхилено' (нічого не зроблено)");
            this.Close();
        }
    }
}