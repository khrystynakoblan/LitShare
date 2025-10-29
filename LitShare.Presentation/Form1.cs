// Додайте ці два рядки вгорі разом з іншими 'using'
using LitShare.BLL.Services;
using System.Windows.Forms;

namespace LitShare.Presentation
{
    public partial class Form1 : Form
    {
        // Створюємо екземпляр нашого сервісу з BLL
        private readonly UserService _userService = new UserService();

        public Form1()
        {
            InitializeComponent();

            // Можете змінити текст на кнопці та назву форми тут
            button1.Text = "Завантажити Користувачів";
            this.Text = "Перевірка Бази Даних";
        }

        // Цей метод у вас з'явився автоматично
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. ЗВЕРТАЄМОСЬ ДО BLL:
                // Викликаємо метод, який ви написали в UserService
                var allUsers = _userService.GetAllUsers();

                // 2. ПОКАЗУЄМО ДАНІ В UI:
                // Прив'язуємо список користувачів до нашої таблиці DataGridView
                // (Припустимо, ваша таблиця називається 'dataGridView1')
                dataGridView1.DataSource = allUsers;

                MessageBox.Show($"Успішно завантажено {allUsers.Count} користувачів!");
            }
            catch (Exception ex)
            {
                // 3. ПЕРЕХОПЛЕННЯ ПОМИЛКИ:
                // Якщо тут буде помилка (невірний пароль, немає зв'язку з Supabase),
                // ви побачите її у цьому вікні.
                MessageBox.Show($"ПОМИЛКА ПІДКЛЮЧЕННЯ: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}