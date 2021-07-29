using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Updater.Common;
using Updater.Content;
using Updater.Logging;
using Updater.Updating;
using Updater.ViewModels;

namespace Updater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = ViewModel;
            InitializeComponent();
            Title = Properties.Resources.Server;
        }
        
        /*
         * WINDOW
         */
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ContentSystem contentSystem = new ContentSystem(ViewModel, LinksPanel);
            contentSystem.Start();

#if DEBUG
            ViewModel.Label1 = "Debug mode";
            ViewModel.Label2 = "3 / 12";
            ViewModel.Label3 = "5 032 KB / 6 723 KB";
            ViewModel.Progress1 = 5032.0 / 6723.0 * 100.0;
            ViewModel.Progress2 = 3.0 / 12.0 * 100.0;
#else
            IUpdatingSystem filesUpdatingSystem = new FilesUpdatingSystem(ViewModel);
            IUpdatingSystem dataUpdatingSystem = new DataUpdatingSystem(ViewModel);
            UpdatingEngine updatingEngine = new UpdatingEngine(ViewModel, filesUpdatingSystem, dataUpdatingSystem);
            updatingEngine.StartUpdating();
#endif
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // ignored
            }
        }

        private void HideButton_OnClickButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /*
         * START GAME
         */
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        /// <summary>
        /// Начать игру
        /// </summary>
        private void StartGame()
        {
            try
            {
                // Закрыть игру
                Helper.ShutdownProcess("game");
                if (Global.IsGameStarted())
                {
                    string text = Strings.Get(StringType.REQUIRED_CLOSE_THE_GAME);
                    string caption = Strings.Get(StringType.ERROR);
                    MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (string necessaryFile in Global.ClientFiles)
                {
                    string filePath = Path.Combine(Global.ClientPath, necessaryFile);
                    if (!File.Exists(filePath))
                    {
                        string text1 = Strings.Get(StringType.NO_REQUIRED_FILES);
                        string text2 = Strings.Get(StringType.FILE_MISSING, necessaryFile);
                        string caption = Strings.Get(StringType.ERROR);
                        MessageBox.Show($"{text1}\n{text2}", caption, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
                // Запуск игры
                ViewModel.Label3 = Strings.Get(StringType.Starting);
                Helper.RunFile(NotifierPath);
                Helper.RunFile(GameExePath, "start");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        
        

        /*
         * OTHER
         */
        /// <summary>
        /// Нажатие на ссылку
        /// </summary>
        private void Link_OnClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FrameworkElement grid = (FrameworkElement)sender;
                string link = grid.Tag.ToString();
                Process.Start("explorer", link);
            }
            catch
            {
                // ignored
            }
        }
        
        

        /// <summary>
        /// 
        /// </summary>
        private MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        private readonly DispatcherTimer _toggleTabTimer = new DispatcherTimer();

        /// <summary>
        /// Название файла game
        /// </summary>
        private static readonly string GameExePath = Path.Combine(Global.ClientPath, Properties.Resources.Game + ".exe");

        private static readonly string NotifierPath = Path.Combine(Global.ClientPath, "notifier.exe");
    }
}
