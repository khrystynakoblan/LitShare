namespace LitShare.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Manages navigation between WPF windows using an internal stack.
    /// </summary>
    public static class NavigationManager
    {
        private static readonly Stack<Window> _windowStack = new Stack<Window>();
        private static Window? _currentWindow;
        private static bool _isNavigating = false;

        /// <summary>
        /// Navigates to a new window and hides the current one.
        /// </summary>
        public static void NavigateTo(Window newWindow, Window currentWindow)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                if (currentWindow != null)
                {
                    _windowStack.Push(currentWindow);
                    currentWindow.Hide();
                }

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
        /// Opens a modal dialog and restores the owner window afterward.
        /// </summary>
        public static bool? ShowDialog(Window dialog, Window owner)
        {
            owner.Hide();
            dialog.Owner = owner;

            bool? result = dialog.ShowDialog();

            owner.Show();

            return result;
        }

        /// <summary>
        /// Returns to the previous window in the navigation stack.
        /// </summary>
        public static void GoBack()
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                if (_currentWindow != null)
                {
                    _currentWindow.Closed -= OnWindowClosed;
                    _currentWindow.Close();
                    _currentWindow = null;
                }

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
        /// Clears the navigation stack and opens the main page.
        /// </summary>
        public static void GoToMainPage(int userId)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                var windowsToClose = new List<Window>();

                if (_currentWindow != null)
                {
                    _currentWindow.Closed -= OnWindowClosed;
                    windowsToClose.Add(_currentWindow);
                    _currentWindow = null;
                }

                while (_windowStack.Count > 0)
                {
                    var window = _windowStack.Pop();
                    windowsToClose.Add(window);
                }

                _windowStack.Clear();

                var mainPage = new MainPage(userId);
                _currentWindow = mainPage;
                mainPage.Closed += OnWindowClosed;
                mainPage.Show();

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
        /// Handles window closing and triggers back navigation if needed.
        /// </summary>
        private static void OnWindowClosed(object sender, EventArgs e)
        {
            if (_isNavigating) return;

            if (sender is Window closedWindow)
            {
                closedWindow.Closed -= OnWindowClosed;

                if (closedWindow == _currentWindow)
                {
                    GoBack();
                }
            }
        }
    }
}