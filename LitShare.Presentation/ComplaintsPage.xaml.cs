// -----------------------------------------------------------------------
// <copyright file="ComplaintsPage.xaml.cs" company="LitShare">
// Copyright (c) 2025 LitShare. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.Presentation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;

    /// <summary>
    /// Логіка взаємодії для сторінки (вікна) перегляду списку скарг.
    /// </summary>
    public partial class ComplaintsPage : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintsPage"/> class.
        /// </summary>
        public ComplaintsPage()
        {
            InitializeComponent();
            LoadComplaints();

            // Прив'язка даних після завантаження
            ComplaintsGrid.ItemsSource = Complaints;
        }

        /// <summary>
        /// Gets or sets the collection of complaints to be displayed.
        /// </summary>
        // ВИПРАВЛЕННЯ CS8618: Ініціалізація порожньою колекцією для уникнення null
        public ObservableCollection<ComplaintDto> Complaints { get; set; } = new ObservableCollection<ComplaintDto>();

        private void LoadComplaints()
        {
            try
            {
                var service = new ComplaintsService();
                var complaintsList = service.GetAllComplaints();

                // Оновлюємо колекцію. Оскільки ми ініціалізували її вище, тут ми просто перезаписуємо посилання.
                Complaints = new ObservableCollection<ComplaintDto>(complaintsList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося завантажити список скарг: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComplaintsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ComplaintsGrid.SelectedItem is ComplaintDto selectedComplaint)
            {
                // Примітка: Для повноцінної реалізації відкриття детального вікна (ComplaintReviewWindow),
                // вам потрібно буде додати поле 'Id' в ComplaintDto та метод GetAllComplaints,
                // щоб передати цей ID у конструктор ComplaintReviewWindow(int complaintId).

                MessageBox.Show(
                    $"Перегляд скарги на книгу: {selectedComplaint.BookTitle}\nВід користувача: {selectedComplaint.UserName}",
                    "Деталі скарги",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}