using System;
using System.Windows.Media.Imaging;

namespace Updater.Content
{
    public class Banner
    {
        public string Source
        {
            get => _link;
            set
            {
                _link = value;
                Image = new BitmapImage(new Uri(Source, UriKind.Absolute));
            }
        }
        private string _link;
        public string Tooltip { get; set; }
        public string Link { get; set; }

        public BitmapImage Image { get; set; }
    }
}
