using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Updater.ViewModels;

namespace Updater.Content
{
    public class ContentSystem
    {

        public ContentSystem(MainWindowViewModel viewModel, StackPanel linksPanel, TabControl ratingTabControl)
        {
            ViewModel = viewModel;
            LinksPanel = linksPanel;
            RatingTabControl = ratingTabControl;
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
            CheckServerStatus();
            UpdateFactionBalance();
        }

        private async Task LoadContent()
        {
            try
            {
                var news = await UpdaterContent.LoadNews();
                ViewModel.SetNews(news);
                var banners = await UpdaterContent.LoadBanners();
                ViewModel.SetBanners(banners);
                var menu = await UpdaterContent.LoadLinks();
                SetMenu(menu);
                var ratings = await UpdaterContent.LoadRatings();
                SetRatings(ratings);
            }
            catch
            {
                // ignored
            }
        }
        
        /// <summary>
        /// Получить ссылки меню.
        /// </summary>
        private void SetMenu(IEnumerable<MenuItem> menu)
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
        /// Build and show ratings tabs.
        /// </summary>
        private void SetRatings(IEnumerable<Rating> ratings)
        {
            try
            {
                RatingTabControl.Items.Clear();
                foreach (Rating rating in ratings)
                {
                    TabItem tabItem = new TabItem { Margin = new Thickness(0), BorderBrush = new SolidColorBrush(Colors.Transparent) };
                    StackPanel stackPanel = new StackPanel { Background = new SolidColorBrush(Color.FromRgb(0x11, 0x12, 0x16)) }; // 111216
                    Label label = new Label { Content = rating.Name };
                    ListView listView = new ListView { ItemsSource = rating.Players, ItemTemplate = RatingTabControl.Resources["RatingTemplate"] as DataTemplate };

                    // Build content.
                    stackPanel.Children.Add(label);
                    stackPanel.Children.Add(listView);
                    tabItem.Content = stackPanel;
                    RatingTabControl.Items.Add(tabItem);
                }
                RatingTabControl.SelectedIndex = new Random().Next(RatingTabControl.Items.Count);
            }
            catch
            {
                // ignored
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
                bool isOnline = jsonString == "1";
                if (isOnline)
                {
                    ViewModel.ServerOnlineVisibility = Visibility.Visible;
                    ViewModel.ServerOfflineVisibility = Visibility.Collapsed;
                }
                else
                {
                    ViewModel.ServerOnlineVisibility = Visibility.Collapsed;
                    ViewModel.ServerOfflineVisibility = Visibility.Visible;
                }
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

        public TabControl RatingTabControl { get; }

        public StackPanel LinksPanel { get; }

        private readonly DispatcherTimer _timer = new DispatcherTimer();
    }
}
