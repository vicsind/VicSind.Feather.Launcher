using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Updater.ViewModels;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Updater.Content
{
    public class ContentSystem
    {

        public ContentSystem(MainWindowViewModel viewModel, StackPanel linksPanel)
        {
            ViewModel = viewModel;
            LinksPanel = linksPanel;
            _timer.Interval = TimeSpan.FromSeconds(5);
        }

        public async void Start()
        {
            _timer.Tick += TimerOnTick;
            _timer.Start();
            await LoadContent();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            UpdatePlayersOnline();
            UpdateFactionBalance();
            CheckServerStatus();
        }

        private async Task LoadContent()
        {
            try
            {
                var news = await UpdaterContent.LoadNews();
                ViewModel.SetNews(news);
                //var banners = await UpdaterContent.LoadBanners();
                //ViewModel.SetBanners(banners);
                var menu = await UpdaterContent.LoadLinks();
                SetMenu(menu);
                //var actualEvents = await UpdaterContent.LoadEvents();
                //ViewModel.SetActualEvents(actualEvents);

                await UpdatePlayersOnline();
                await UpdateFactionBalance();
                await CheckServerStatus();
            }
            catch
            {
                // ignored
            }
        }
        
        /// <summary>
        /// Получить ссылки меню.
        /// </summary>
        private void SetMenu(List<MenuItem> menu)
        {
            try
            {
                LinksPanel.Children.Clear();
                foreach (MenuItem link in menu)
                {
                    Label label = new Label { Content = link.Name, Tag = link.Url };
                    if (!string.IsNullOrWhiteSpace(link.Foreground))
                    {
                        var color = ColorConverter.ConvertFromString(link.Foreground);
                        if (color is Color foreground)
                            label.Foreground = new SolidColorBrush(foreground);
                    }
                    LinksPanel.Children.Add(label);

                    //if (link != menu.Last())
                    //{
                    //    Rectangle rectangle = new Rectangle
                    //        { Height = 6, Width = 6, Fill = new SolidColorBrush(Color.FromRgb(204, 0, 0)) };
                    //    rectangle.LayoutTransform = new RotateTransform(45);
                    //    LinksPanel.Children.Add(rectangle);
                    //}
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Получить данные о балансе фракций
        /// </summary>
        private async Task UpdateFactionBalance()
        {
            try
            {
                string jsonString = await UpdaterContent.LoadContentFromUrlAsync(UpdaterContent.FactionBalanceUrl);
                if (jsonString == "")
                    return;

                JArray jsonArr = JArray.Parse(jsonString);
                ViewModel.BalanceAoL = (double)jsonArr[0];
                ViewModel.BalanceUoF = (double)jsonArr[1];
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task UpdatePlayersOnline()
        {
            try
            {
                string jsonString = await UpdaterContent.LoadContentFromUrlAsync(UpdaterContent.PlayersOnlineUrl);
                if (jsonString == "")
                    return;

                int playersCount = Convert.ToInt32(jsonString);
                ViewModel.PlayersOnline = playersCount;
            }
            catch
            {
                ViewModel.PlayersOnline = 0;
            }
        }


        /*
         * SERVER CHECKING
         */
        /// <summary>
        /// Проверить сервер на доступность
        /// </summary>
        private async Task CheckServerStatus()
        {
            try
            {
                string jsonString = await UpdaterContent.LoadContentFromUrlAsync(UpdaterContent.ServerStatusUrl);
                ViewModel.IsServerOnline = jsonString == "1";
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private MainWindowViewModel ViewModel { get; }
        
        public StackPanel LinksPanel { get; }

        private readonly DispatcherTimer _timer = new DispatcherTimer();
    }
}
