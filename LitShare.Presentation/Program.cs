using System;
using System.Windows.Forms;

namespace LitShare.Presentation
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Цей рядок запускає ваше головне вікно Form1
            Application.Run(new Form1());
        }
    }
}