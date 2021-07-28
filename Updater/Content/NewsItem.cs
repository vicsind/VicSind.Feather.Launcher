using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Updater.Content
{
    public class NewsItem
    {
        public string ImgUrl
        {
            get => _link;
            set
            {
                _link = value;
                Thumbnail = new BitmapImage(new Uri(ImgUrl, UriKind.Absolute));
            }
        }
        private string _link;

        public BitmapImage Thumbnail { get; set; }

        public string Title { get; set; }
        public string Desc { get; set; }

        public string LinkUrl { get; set; }
        public DateTime Date { get; set; }

        public string DateLabel
        {
            get
            {
                if (Date == DateTime.Today)
                    return "Today";
                if (Date == DateTime.Today.AddDays(-1))
                    return "Yesterday";
                return Date.ToString("d MMM yyyy", new CultureInfo("au-EN"));
            }
        }
        public string Button { get; set; } = "READ MORE";
        public Visibility ButtonVisibility => string.IsNullOrEmpty(LinkUrl) ? Visibility.Collapsed : Visibility.Visible;
    }
}
