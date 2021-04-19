using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Updater.Common;
using Updater.Content;
using Updater.ViewModels;

namespace Updater.Updating
{
    public class FilesUpdatingSystem : IUpdatingSystem
    {
        private readonly IProgressViewModel _viewModel;
        public FilesUpdatingSystem(IProgressViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        /// <inheritdoc cref="IUpdatingSystem"/>
        public event EventHandler OnCompleted;

        /// <inheritdoc cref="IUpdatingSystem"/>
        public void Start()
        {
            _viewModel.Label1 = Strings.Get(StringType.UpdatingClientFiles); // "Обновление файлов клиента"
            // Get actual files info.
            string content = UpdaterContent.LoadContentFromUrl(ClientFilesUrl);
            _actualFiles = JsonConvert.DeserializeObject<ClientFile[]>(content) ?? new ClientFile[0];
            _checkFilesQueue = new Queue<ClientFile>(_actualFiles);
            
            // Client files.
            Global.ClientFiles = _actualFiles.Select(x => x.Name)
                .Concat(new[] { Global.SahPath, Global.SafPath })
                .ToArray();

            // Starts to checking files.
            CheckNextFile();
        }

        private void CheckNextFile()
        {
            // Checking finished.
            if (_checkFilesQueue.Count == 0)
            {
                RemoveBanFiles();
                OnCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }

            ClientFile actualFile = _checkFilesQueue.Dequeue();
            // Updating required when target file not exists or checksum mismatched with current file.
            string filePath = Path.Combine(Global.ClientPath, actualFile.Name);
            bool updatingRequired = !File.Exists(filePath)
                                    || !string.IsNullOrEmpty(actualFile.Checksum) 
                                    && !Helper.Checksum(filePath, actualFile.Checksum);
            if (updatingRequired)
            {
                bool isNewLauncher = string.Equals(actualFile.Name, Global.UPDATER_FILENAME, StringComparison.OrdinalIgnoreCase);
                if (isNewLauncher)
                {
                    DownloadLauncher(actualFile);
                }
                else
                {
                    DownloadFile(actualFile);
                }
            }
            else
            {
                CheckNextFile();
            }
        }

        private void DownloadFile(ClientFile actualFile)
        {
            Thread.Sleep(500);
            // Remove current file.
            string filePath = Path.Combine(Global.ClientPath, actualFile.Name);
            Helper.DeleteFile(filePath);
            //MessageBox.Show($"Downloading {actualFile.Name} to \n{filePath}");

            // Download new file.
            _viewModel.Label1 = Strings.Get(StringType.Downloading); // "Загрузка"
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += (sender, args) =>
            {
                _viewModel.Progress1 = args.ProgressPercentage;
                _viewModel.Progress2 = args.ProgressPercentage;
            };
            webClient.DownloadFileCompleted += (sender, args) => CheckNextFile();

            var fileUrl = new Uri(Path.Combine(FilesUrl, actualFile.Name));
            webClient.DownloadFileAsync(fileUrl, filePath);
        }

        /// <summary>
        /// Начать скачивание апдейтера для последующего открытия
        /// </summary>
        private void DownloadLauncher(ClientFile actualFile)
        {
            Thread.Sleep(500);
            //MessageBox.Show($"Downloading {actualFile.Name} to \n{Common.TempUpdaterPath}");
            // Download new launcher to temp file.
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += (sender, args) =>
            {
                _viewModel.Progress1 = args.ProgressPercentage;
                _viewModel.Progress2 = args.ProgressPercentage;
            };
            webClient.DownloadFileCompleted += (sender, args) =>
            {
                Helper.RunFile(Global.TempUpdaterPath, "/u");
                Environment.Exit(0); 
            };

            var fileUrl = new Uri(Path.Combine(FilesUrl, actualFile.Name));
            webClient.DownloadFileAsync(fileUrl, Global.TempUpdaterPath);
        }

        /// <summary>
        /// Remove all banned files in client folder.
        /// </summary>
        private static void RemoveBanFiles()
        {
            try
            {
                // Get ban files info.
                string content = UpdaterContent.LoadContentFromUrl(BanFilesUrl);
                string[] banFiles = JsonConvert.DeserializeObject<string[]>(content) ?? new string[0];
                foreach (string banFile in banFiles)
                {
                    foreach (string fileToRemove in Directory.GetFiles(Global.ClientPath, banFile))
                    {
                        Helper.DeleteFile(fileToRemove);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
        
        /// <summary>
        /// Information about actual client files.
        /// </summary>
        private ClientFile[] _actualFiles;

        /// <summary>
        /// 
        /// </summary>
        private Queue<ClientFile> _checkFilesQueue;

        /// <summary>
        /// WebSite url with information about actual client files.
        /// </summary>
        private static string ClientFilesUrl => Environment.Is64BitOperatingSystem ? Global.SiteUrl + "files-x64" : Global.SiteUrl + "files";

        /// <summary>
        /// WebSite url with information about banned files.
        /// </summary>
        private static string BanFilesUrl => Global.SiteUrl + "ban-files";

        /// <summary>
        /// Url to actual client files.
        /// </summary>
        private static string FilesUrl => Environment.Is64BitOperatingSystem ? Global.SiteUrl + "client-x64/" : Global.SiteUrl + "client/";
    }
}
