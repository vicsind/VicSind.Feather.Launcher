using System;
using System.IO;
using System.Net;
using System.Threading;
using Ionic.Zip;
using Updater.Common;
using Updater.Content;
using Updater.Logging;
using Updater.SAH;
using Updater.ViewModels;

namespace Updater.Updating
{
    public class DataUpdatingSystem : IUpdatingSystem
    {
        private readonly IProgressViewModel _viewModel;
        public DataUpdatingSystem(IProgressViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        /// <summary>
        /// SAH
        /// </summary>
        public SahData SahFile;

        /// <inheritdoc cref="IUpdatingSystem"/>
        public event EventHandler OnCompleted;

        /// <inheritdoc cref="IUpdatingSystem"/>
        public void Start()
        {
            // Load actual version.
            string content = UpdaterContent.LoadContentFromUrl(VersionUrl);
            _actualVersion = int.TryParse(content, out int version) ? version : 0;
            // Load local version.
            LoadLocalVersion();

            // Загрузить SAH
            SahFile = new SahData(Global.SahPath);
            SahFile.Load();

            _currentFileIndex = 1;
            _totalFileCount = _actualVersion - _currentVersion;

            _totalSize = 0;
            _viewModel.Progress1 = 0;
            _viewModel.Progress2 = 0;

            // Получить общий размер скачиваемых файлов
            for (int i = _currentVersion + 1; i <= _actualVersion; i++)
            {
                try
                {
                    WebRequest request = WebRequest.Create($"{DataFilesUrl}upd{i}.zip");
                    request.Method = "HEAD";
                    WebResponse response = request.GetResponse();
                    _totalSize += (int)response.ContentLength / 1024;
                    request.Abort();
                }
                catch
                {
                    _viewModel.Label1 = Strings.Get(StringType.FileNotFoundOnServer, $"upd{i}.zip"); // "Файл UpdX.zip не найден на сервере"
                    return;
                }
            }

            // Начать загрузку патча
            StartDownloading();
        }

        /// <summary>
        /// Загрузка файлов с сервера и последующая распаковка
        /// </summary>
        private void StartDownloading()
        {
            // Обновление завершено
            if (_currentVersion >= _actualVersion)
            {
                OnCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            _viewModel.Label1 = Strings.Get(StringType.Downloading); // "Загрузка"
            _viewModel.Label2 = $"{_currentFileIndex} / {_totalFileCount}";
            // Удалить временные файлы
            if (!Helper.DeleteFile(PatchPath) || !Helper.DeleteDir(PatchContentFolder))
            {
                _viewModel.Label1 = $"{Strings.Get(StringType.UpdatingError)} 302"; // "Ошибка"
                return;
            }

            // Начать скачивание
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += WebClient_OnDownloadProgressChanged;
                webClient.DownloadFileCompleted += (sender, args) => OnFileDownloaded();
                webClient.DownloadFileAsync(new Uri($"{DataFilesUrl}upd{_currentVersion + 1}.zip"), PatchPath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Действие после скачки файла
        /// </summary>
        private void OnFileDownloaded()
        {
            _viewModel.Label1 = $"{Strings.Get(StringType.Unpacking)}"; // "Распаковка файла"

            // Размер скаченных файлов
            _downloaded += _currentFileSize;
            _currentFileIndex++;

            try
            {
                // Распаковка zip
                using (ZipFile archive = new ZipFile(PatchPath))
                    archive.ExtractAll(PatchContentFolder, ExtractExistingFileAction.OverwriteSilently);

                // Удалить временный файл
                Helper.DeleteFile(PatchPath);

                ///////////////////
                // Старт обновления клиента
                _viewModel.Label1 = $"{Strings.Get(StringType.Updating)}"; // "Обновление"
                DirectoryInfo patchFolder = new DirectoryInfo(PatchContentFolder);
                if (!patchFolder.Exists)
                    throw new Exception("Patch directory not founded");

                using (FileStream safStream = File.Open(Global.SafPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    SahFile.Patch(safStream, patchFolder);
                SahFile.Save();
                ///////////////////

                // Удалить временный каталог
                Helper.DeleteDir(PatchContentFolder);

                // Сохранение текущей версии.
                _currentVersion += 1;
                File.WriteAllText(VersionPath, _currentVersion.ToString());
            }
            catch (Exception ex)
            {
                _viewModel.Label1 = $"{Strings.Get(StringType.UpdatingError)} 301"; // "Загрузка"
                Logger.LogError(ex);
                return;
            }

            Thread.Sleep(1000);
            StartDownloading();
        }

        /// <summary>
        /// Прогресс скачивания файла
        /// </summary>
        private void WebClient_OnDownloadProgressChanged(object o, DownloadProgressChangedEventArgs args)
        {
            double totalDownloaded = _downloaded + args.BytesReceived / 1024.0;
            double percentage2 = totalDownloaded / _totalSize * 100;
            _viewModel.Progress1 = args.ProgressPercentage;
            _viewModel.Progress2 = percentage2;
            _viewModel.Label3 = $"{totalDownloaded:N0} KB / {_totalSize:N0} KB";

            _currentFileSize = (int)args.TotalBytesToReceive / 1024;
        }


        private void LoadLocalVersion()
        {
            try
            {
                string content = File.ReadAllText(VersionPath);
                _currentVersion = int.TryParse(content, out int version) ? version : 0;
            }
            catch
            {
                _currentVersion = 0;
            }
        }

        /// <summary>
        /// Текущая версия клиента.
        /// </summary>
        private int _currentVersion;

        /// <summary>
        /// Актуальная версия клиента.
        /// </summary>
        private int _actualVersion;

        /// <summary>
        /// 
        /// </summary>
        private int _totalSize = 100;

        /// <summary>
        /// Текущий индекс скачиваемого файла
        /// </summary>
        private int _currentFileIndex = 1;

        /// <summary>
        /// Общее количество файлов
        /// </summary>
        private int _totalFileCount;

        /// <summary>
        /// Размер качаемого файла
        /// </summary>
        private int _currentFileSize;

        /// <summary>
        /// Размер файлов, которые были скачены
        /// </summary>
        private int _downloaded;

        /// <summary>
        /// Название version файла
        /// </summary>
        private static readonly string VersionPath = Path.Combine(Global.ClientPath, "version");

        /// <summary>
        /// Путь к данным о версии клиента
        /// </summary>
        private static string VersionUrl => Global.SiteUrl + "version";

        /// <summary>
        /// Путь к каталогу обновлений клиента
        /// </summary>
        public static string DataFilesUrl => Global.SiteUrl + "data/";

        /// <summary>
        /// Путь к временной папке
        /// </summary>
        private static string PatchContentFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "imperium");

        /// <summary>
        /// Путь к патчу
        /// </summary>
        public static string PatchPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "imperium-patch.zip");
    }
}
