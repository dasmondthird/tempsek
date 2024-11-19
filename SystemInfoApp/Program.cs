using System;
using System.Windows.Forms;

namespace SystemInfoApp
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Запуск основной формы
            Application.Run(new MainForm());
        }
    }
}
