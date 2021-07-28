using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Updater.Common;

namespace Updater.Content
{
    public static class UpdaterContent
    {
        public static string LoadContentFromUrl(string url)
        {
            using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
                return client.DownloadString(url);
        }

        public static async Task<string> LoadContentFromUrlAsync(string url)
        {
            using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
                return await client.DownloadStringTaskAsync(url);
        }

        /// <summary>
        /// Получить ссылки меню.
        /// </summary>
        public static async Task<List<MenuItem>> LoadLinks()
        {
            try
            {
                string jsonString = await LoadContentFromUrlAsync(MenuUrl);
                return JsonConvert.DeserializeObject<List<MenuItem>>(jsonString);
            }
            catch
            {
                return new List<MenuItem>();
            }
        }

        /// <summary>
        /// Получить с сервера новости
        /// </summary>
        public static async Task<List<NewsItem>> LoadNews()
        {
            try
            {
                string jsonString = await LoadContentFromUrlAsync(NewsUrl);
                return JsonConvert.DeserializeObject<List<NewsItem>>(jsonString);
            }
            catch
            {
                return new List<NewsItem>();
            }
        }
        
        /// <summary>
        /// Загрузить данные о баннерах.
        /// </summary>
        public static async Task<List<Banner>> LoadBanners()
        {
            try
            {
                string jsonString = await LoadContentFromUrlAsync(BannersUrl);
                return JsonConvert.DeserializeObject<List<Banner>>(jsonString);
            }
            catch
            {
                return new List<Banner>();
            }
        }


        /// <summary>
        /// Загрузить данные об актуальных ивентах.
        /// </summary>
        public static async Task<List<Rating>> LoadEvents()
        {
            try
            {
                string jsonString = await LoadContentFromUrlAsync(RatingsUrl);
                return JsonConvert.DeserializeObject<List<Rating>>(jsonString);
            }
            catch
            {
                return new List<Rating>();
            }
        }

        /// <summary>
        /// Путь к данным о ссылках меню
        /// </summary>
        public static string MenuUrl => Global.SiteUrl + "menu";

        /// <summary>
        /// Путь к данным о новостях
        /// </summary>
        public static string NewsUrl => Global.SiteUrl + "news";

        /// <summary>
        /// Путь к данным о новостях
        /// </summary>
        public static string FactionBalanceUrl => Global.SiteUrl + "faction-balance";

        /// <summary>
        /// Путь к данным о рейтингах.
        /// </summary>
        public static string RatingsUrl => Global.SiteUrl + "ratings";

        /// <summary>
        /// Путь к данным о баннерах.
        /// </summary>
        public static string BannersUrl => Global.SiteUrl + "banners";

        /// <summary>
        /// URL статуса сервера.
        /// </summary>
        public static string ServerStatusUrl => Global.SiteUrl + "server-status";

        /// <summary>
        /// 
        /// </summary>
        public static string FWindowsUrl => Global.SiteUrl + "f-windows";
        public static string FProcessesUrl => Global.SiteUrl + "f-processes";
    }
}
