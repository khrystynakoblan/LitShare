// NavigationManager.cs - ВИПРАВЛЕНА ВЕРСІЯ
namespace LitShare.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public static class NavigationManager
    {
        private static readonly Stack<Window> _windowStack = new Stack<Window>();
        private static Window? _currentWindow;
        private static bool _isNavigating = false;

        /// <summary>
        /// Відкриває нове вікно та ховає поточне
        /// </summary>
        public static void NavigateTo(Window newWindow, Window currentWindow)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                // Додаємо поточне вікно в стек
                if (currentWindow != null)
                {
                    _windowStack.Push(currentWindow);
                    currentWindow.Hide();
                }

                // Налаштовуємо нове вікно
                _currentWindow = newWindow;
                newWindow.Closed += OnWindowClosed;
                newWindow.Show();
            }
            finally
            {
                _isNavigating = false;
            }
        }

        /// <summary>
        /// Відкриває діалогове вікно
        /// </summary>
        public static bool? ShowDialog(Window dialog, Window owner)
        {
            owner.Hide();
            dialog.Owner = owner;

            bool? result = dialog.ShowDialog();

            // Після закриття діалогового вікна — повертаємо головне
            owner.Show();

            return result;
        }


        /// <summary>
        /// Повертається до попереднього вікна
        /// </summary>
        public static void GoBack()
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                // Закриваємо поточне вікно
                if (_currentWindow != null)
                {
                    _currentWindow.Closed -= OnWindowClosed;
                    _currentWindow.Close();
                    _currentWindow = null;
                }

                // Відкриваємо попереднє вікно зі стеку
                if (_windowStack.Count > 0)
                {
                    var previousWindow = _windowStack.Pop();
                    _currentWindow = previousWindow;
                    _currentWindow.Show();
                }
            }
            finally
            {
                _isNavigating = false;
            }
        }

        /// <summary>
        /// Повертається до головної сторінки
        /// </summary>
        public static void GoToMainPage(int userId)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                // Збираємо всі вікна, які потрібно закрити (крім нового головного)
                var windowsToClose = new List<Window>();

                // Додаємо поточне вікно
                if (_currentWindow != null)
                {
                    _currentWindow.Closed -= OnWindowClosed;
                    windowsToClose.Add(_currentWindow);
                    _currentWindow = null;
                }

                // Додаємо всі вікна зі стеку
                while (_windowStack.Count > 0)
                {
                    var window = _windowStack.Pop();
                    windowsToClose.Add(window);
                }

                // Очищаємо стек
                _windowStack.Clear();

                // Спочатку створюємо нову головну сторінку
                var mainPage = new MainPage(userId);
                _currentWindow = mainPage;
                mainPage.Closed += OnWindowClosed;
                mainPage.Show();

                // Потім закриваємо всі старі вікна
                foreach (var window in windowsToClose)
                {
                    if (window.IsVisible && window != mainPage)
                    {
                        window.Close();
                    }
                }
            }
            finally
            {
                _isNavigating = false;
            }
        }

        /// <summary>
        /// Обробник закриття вікна
        /// </summary>
        private static void OnWindowClosed(object sender, EventArgs e)
        {
            if (_isNavigating) return;

            if (sender is Window closedWindow)
            {
                closedWindow.Closed -= OnWindowClosed;

                // Якщо це поточне вікно, викликаємо GoBack
                if (closedWindow == _currentWindow)
                {
                    GoBack();
                }
            }
        }
    }
}