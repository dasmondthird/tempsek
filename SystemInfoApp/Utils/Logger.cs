using System;
using System.IO;

namespace SystemInfoApp.Utils
{
    public class Logger : IDisposable
    {
        private readonly string _logFilePath;
        private readonly object _lock = new object();
        private StreamWriter? _writer;

        public Logger(string logFileName)
        {
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SystemInfoApp_Logs");
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, logFileName);
            _writer = new StreamWriter(_logFilePath, append: true) { AutoFlush = true };
        }

        public void Log(string message)
        {
            lock (_lock)
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                _writer?.WriteLine(logMessage);
                Console.WriteLine(logMessage); // Дополнительно выводим в консоль
            }
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}
