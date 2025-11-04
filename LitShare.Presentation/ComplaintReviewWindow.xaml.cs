using LitShare.BLL.Services; // 1. ДОДАНО
using LitShare.DAL.Models;   // 2. ДОДАНО
using System.Threading.Tasks;  // 3. ДОДАНО
using System.Windows;

namespace LitShare.Presentation
{
    public partial class ComplaintReviewWindow : Window
    {
        // 4. ДОДАНО: Сервіси
        private readonly ComplaintsService _complaintsService = new ComplaintsService();
        private readonly UserService _userService = new UserService(); // Потрібен для імені автора

        // 5. ДОДАНО: Поле для збереження ID
        private int _currentComplaintId;

        // 6. 🔹 ЗМІНЕNO: Конструктор тепер БЕЗ ПАРАМЕТРІВ (для тесту)
        public ComplaintReviewWindow()
        {
            InitializeComponent();

            // 🔹 7. ДОДАНО: Жорстко задаємо тестовий ID
            int testComplaintId = 1; // <--- ПОСТАВ ТУТ ID СКАРГИ, ЯКА 100% ІСНУЄ В ТВОЇЙ БД

            _currentComplaintId = testComplaintId; // Зберігаємо ID

            // 8. Викликаємо завантаження
            _ = LoadComplaintDataAsync(_currentComplaintId);
        }

        // 9. Метод завантаження даних (залишається як є)
        private async Task LoadComplaintDataAsync(int complaintId)
        {
            try
            {
                // Потрібно, щоб у ComplaintsService був метод GetComplaintWithDetails
                var complaint = _complaintsService.GetComplaintWithDetails(complaintId);

                if (complaint != null && complaint.Post != null)
                {
                    // Заповнюємо поля
                    txtComplaintReason.Text = complaint.text;
                    txtPostTitle.Text = complaint.Post.title;
                    txtPostDescription.Text = complaint.Post.description;

                    // Треба вручну завантажити автора оголошення
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


        // --- Кнопки без функціоналу (просто закривають вікно) ---

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Просто закриваємо
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