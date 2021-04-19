﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            ContentSystem contentSystem = new ContentSystem(ViewModel, LinksPanel, RatingTabControl);
            contentSystem.Start();

            ToggleTabTimer.Tick += (o, args) => ToggleTab();
            ToggleTabTimer.Interval = TimeSpan.FromSeconds(5);
            ToggleTabTimer.Start();

#if DEBUG
#else
            IUpdatingSystem filesUpdatingSystem = new FilesUpdatingSystem(ViewModel);
            IUpdatingSystem dataUpdatingSystem = new DataUpdatingSystem(ViewModel);
            UpdatingEngine updatingEngine = new UpdatingEngine(ViewModel, filesUpdatingSystem, dataUpdatingSystem);
            updatingEngine.StartUpdating();
#endif
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
                Helper.ShutdownGame();
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
                Helper.RunFile(GameExePath, "start");

                // Скрыть окно
                WindowState = WindowState.Minimized;
                Hide();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        /*
         * INTERACTIVITY
         */
        /// <summary>
        /// Сменить таб на следующий.
        /// </summary>
        private void ToggleTab()
        {
            int newIndex = RatingTabControl.SelectedIndex == RatingTabControl.Items.Count - 1
                ? 0
                : RatingTabControl.SelectedIndex + 1;
            RatingTabControl.SelectedIndex = newIndex;
        }
        
        /// <summary>
        /// Загрузить контент апдейтера.
        /// </summary>
        private async void StartTickTimer()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(5000);
                    ToggleTab();
                }
                catch
                {
                    // ignored
                }
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

        private DispatcherTimer ToggleTabTimer = new DispatcherTimer();

        /// <summary>
        /// Название файла game
        /// </summary>
        private static readonly string GameExePath = Path.Combine(Global.ClientPath, Properties.Resources.Game + ".exe");
    }
}
