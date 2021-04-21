using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Updater.Common;
using Updater.Logging;

namespace Updater
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Current Application ID.
        /// </summary>
        private const string GUID = "e038fab3-b6c3-4743-a2dc-a6675f803645";

        [STAThread]
        public static void Main(string[] args)
        {
            CloseOpenedUpdater();
            HandleArgs();

            Mutex mutex = new Mutex(true, GUID);
            if (args.Contains("/u") || mutex.WaitOne(TimeSpan.Zero, true))
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
                mutex.ReleaseMutex();
            }
        }

        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, args) => Logger.LogError((Exception)args.ExceptionObject); // Глобальный отлов ошибок
        }

        /// <summary>
        /// Closes opened updater from other locations.
        /// </summary>
        private static void CloseOpenedUpdater()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                var process = Process.GetProcessesByName(currentProcess.ProcessName).FirstOrDefault(x => x.Id != currentProcess.Id);
                if (process != null && process.MainModule?.FileName != currentProcess.MainModule?.FileName)
                {
                    Thread.Sleep(500);
                    process.Kill();
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Обработать аргументы.
        /// </summary>
        private static void HandleArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length <= 1)
                return;

            switch (args[1])
            {
                case "/u": // Копируем новый апдейтер на старое место
                    CopyUpdater();
                    break;
                case "/d": // Удаление временного файла апдейтера
                    Thread.Sleep(1000);
                    Helper.DeleteFile(Global.TempUpdaterPath);
                    break;
            }
        }

        /// <summary>
        /// Скопировать новый апдейтер на место старого
        /// </summary>
        private static void CopyUpdater()
        {
            try
            {
                Thread.Sleep(1000);
                // Удаляем старый апдейтер
                if (!Helper.DeleteFile(UpdaterPath))
                    throw new Exception($"Can't delete {Global.UPDATER_FILENAME}");

                // Копируем скачанный файл в оригинальное имя файла
                File.Copy(Global.ExecutableLocation, UpdaterPath);
                Thread.Sleep(1000);

                // Запускаем апдейтер с флагом на удаление временного файла.
                Helper.RunFile(UpdaterPath, "/d");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            // Закрыть текущий апдейтер.
            Environment.Exit(0);
        }

        /// <summary>
        /// Path to updater.
        /// </summary>
        private static string UpdaterPath => Path.Combine(Global.ClientPath, Global.UPDATER_FILENAME);
    }
}
