using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Updater.Content;

namespace Updater.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase, IProgressViewModel
    {
        public void SetNews(IEnumerable<NewsItem> news)
        {
            News.Clear();
            foreach (NewsItem newsItem in news)
            {
                News.Add(newsItem);
            }
            //dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            //{
            //    News.Clear();
            //    foreach (NewsItem newsItem in news)
            //        News.Add(newsItem);
            //});
        }

        public void SetBanners(IEnumerable<Banner> banners)
        {
            Banners.Clear();
            foreach (Banner banner in banners)
                Banners.Add(banner);
            //dispatcher.BeginInvoke(() =>
            //{
            //    Banners.Clear();
            //    foreach (Banner banner in banners)
            //        Banners.Add(banner);
            //});
        }

        /// <summary>
        /// Баннеры.
        /// </summary>
        public ObservableCollection<Banner> Banners { get; set; } = new ObservableCollection<Banner>();


        public double BalanceAoL
        {
            get => _balanceAoL;
            set
            {
                _balanceAoL = value / 100.0;
                AoL = new GridLength(value, GridUnitType.Star);

                RaisePropertyChanged(nameof(BalanceAoL));
                RaisePropertyChanged(nameof(AoL));
            }
        }
        private double _balanceAoL = 0.5;

        /// <summary>
        /// Длина ширины столбца AoL
        /// </summary>

        public GridLength AoL { get; set; } = new GridLength(0.5, GridUnitType.Star);


        public double BalanceUoF
        {
            get => _balanceUoF;
            set
            {
                _balanceUoF = value / 100.0;
                UoF = new GridLength(value, GridUnitType.Star);

                RaisePropertyChanged(nameof(BalanceUoF));
                RaisePropertyChanged(nameof(UoF));
            }
        }
        private double _balanceUoF = 0.5;

        /// <summary>
        /// Длина ширины столбца UoF
        /// </summary>

        public GridLength UoF { get; set; } = new GridLength(0.5, GridUnitType.Star);


        /// <summary>
        /// 
        /// </summary>
        public Visibility ServerOnlineVisibility
        {
            get => _serverOnlineVisibility;
            set
            {
                _serverOnlineVisibility = value;
                RaisePropertyChanged(nameof(ServerOnlineVisibility));
            }
        }
        private Visibility _serverOnlineVisibility = Visibility.Collapsed;

        /// <summary>
        /// 
        /// </summary>
        public Visibility ServerOfflineVisibility
        {
            get => _serverOfflineVisibility;
            set
            {
                _serverOfflineVisibility = value;
                RaisePropertyChanged(nameof(ServerOfflineVisibility));
            }
        }
        private Visibility _serverOfflineVisibility = Visibility.Collapsed;


        /// <summary>
        /// Новости.
        /// </summary>
        public ObservableCollection<NewsItem> News { get; set; } = new ObservableCollection<NewsItem>();


        /// <summary>
        /// Текстовая метка 1
        /// </summary>
        public string Label1
        {
            get => _label1;
            set
            {
                _label1 = value;
                RaisePropertyChanged(nameof(Label1));
            }
        }
        private string _label1;

        /// <summary>
        /// Текстовая метка 2
        /// </summary>
        public string Label2
        {
            get => _label2;
            set
            {
                _label2 = value;
                RaisePropertyChanged(nameof(Label2));
            }
        }
        private string _label2;

        /// <summary>
        /// Текстовая метка 3
        /// </summary>
        public string Label3
        {
            get => _label3;
            set
            {
                _label3 = value;
                RaisePropertyChanged(nameof(Label3));
            }
        }
        private string _label3;

        /// <summary>
        /// 
        /// </summary>
        #region ProgressValue1

        public double Progress1
        {
            get => _progress1;
            set
            {
                _progress1 = value;
                RaisePropertyChanged(nameof(Progress1));
                RaisePropertyChanged(nameof(ProgressLight));
            }
        }

        private double _progress1 = 100;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        #region ProgressValue2

        public double Progress2
        {
            get => _progress2;
            set
            {
                _progress2 = value;
                RaisePropertyChanged(nameof(Progress2));
                RaisePropertyChanged(nameof(ProgressDark));
            }
        }

        private double _progress2 = 100;
        #endregion

        /// <summary>
        /// Ширина прогресс бара
        /// </summary>
        private const double PROGRESS_BAR_WIDTH = 850.0;

        /// <summary>
        /// Значение ширины первого прогрессбара
        /// </summary>
        public int ProgressLight => (int)(PROGRESS_BAR_WIDTH / 100.0 * Progress1);

        /// <summary>
        /// Значение ширины второго прогрессбара
        /// </summary>
        public int ProgressDark => (int)(PROGRESS_BAR_WIDTH / 100.0 * Progress2);

        public bool IsStartGameEnabled
        {
            get => _IsStartGameEnabled;
            set
            {
                _IsStartGameEnabled = value;
                RaisePropertyChanged(nameof(IsStartGameEnabled));
            }
        }
        private bool _IsStartGameEnabled = true;
    }
}
