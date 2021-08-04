using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Updater.Common
{
    public static class Global
    {
        /// <summary>
        /// Игра запущена в данный момент.
        /// </summary>
        public static bool IsGameStarted() => Process.GetProcessesByName("game").Length != 0;

        /// <summary>
        /// Location of updater file.
        /// </summary>
        public static string ExecutableLocation => Process.GetCurrentProcess().MainModule?.FileName ??
                                                   throw new InvalidOperationException("Executable location is null");

        /// <summary>
        /// Current executable file directory.
        /// </summary>
        public static string ClientPath => Path.GetDirectoryName(ExecutableLocation);


        private static readonly string[] SiteUrls =
        {
            "https://api.amazing-shaiya.com",
            //"http://updater.shaiya-erection.ru/",
            //"https://localhost:44313/"
        };

        /// <summary>
        /// Путь к сайту
        /// </summary>
        public static string SiteUrl
        {
            get
            {
                return SiteUrls.First();
                //if (string.IsNullOrEmpty(_siteUrl))
                //{
                //    foreach (string url in SiteUrls)
                //    {
                //        try
                //        {
                //            new WebClient().OpenRead(url);
                //            // Success
                //            _siteUrl = url;
                //            return url;
                //        }
                //        catch
                //        {
                //            // ignored
                //        }
                //    }
                //}

                //_siteUrl = SiteUrls.First();
                //return _siteUrl;
            }
        }
        private static string _siteUrl;

        
        /// <summary>
        /// Путь к дата файлу без расширения
        /// </summary>
        public static readonly string DataPath = Path.Combine(ClientPath, Properties.Resources.Data);
        
        /// <summary>
        /// Расширение sah файла
        /// </summary>
        private const string SAH_EXTENSION = ".sah";

        /// <summary>
        /// Расширение saf файла
        /// </summary>
        private const string SAF_EXTENSION = ".saf";

        /// <summary>
        /// Path to SAH file.
        /// </summary>
        public static readonly string SahPath = DataPath + SAH_EXTENSION;

        /// <summary>
        /// Path to SAF file.
        /// </summary>
        public static readonly string SafPath = DataPath + SAF_EXTENSION;


        /// <summary>
        /// Файлы, необходимые для запуска
        /// </summary>
        public static string[] ClientFiles;
        

        /// <summary>
        /// Path to updater temp file.
        /// </summary>
        public static string TempUpdaterPath => Path.Combine(ClientPath, TEMP_UPDATER_FILENAME);
        
        /// <summary>
        /// Название файла апдейтера
        /// </summary>
        public const string UPDATER_FILENAME = "Updater.exe";

        /// <summary>
        /// Название файла апдейтера для замены
        /// </summary>
        public const string TEMP_UPDATER_FILENAME = "UpdaterTemp.exe";
    }
}
