using System;
using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Updater.Common;

namespace Updater.Logging
{
    public static class Logger
    {
        /// <summary>
        /// Логирование ошибки
        /// </summary>
        public static void LogError(Exception ex)
        {
            try
            {
                const string filename = "ERROR.log";
                SendError(ex);
                File.WriteAllText(Path.Combine(Global.ClientPath, filename), ex.ToString());
                string text1 = Strings.Get(StringType.PLEASE_SEND_FILE, filename);
                string text2 = Strings.Get(StringType.ERROR_MESSAGE, ex.Message);
                string caption = Strings.Get(StringType.ERROR);
                MessageBox.Show($"{text1}\n{text2}", caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                // ignored
            }
        }

        private static void SendError(Exception ex)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(ErrorUrl);
                request.Method = "POST";
                request.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var errorData = new ErrorData 
                    {
                        Message = ex.ToString(), 
                        PathToExecutable = Global.ExecutableLocation,
                        OSVersion = GetOSName()
                    };
                    var json = JsonConvert.SerializeObject(errorData);
                    streamWriter.Write(json);
                }

                request.GetResponse();
            }
            catch
            {
                // ignored
            }
        }

        public static string GetRegistryValue(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                return (string)rk?.GetValue(key) ?? "";
            }
            catch
            {
                return "";
            }
        }

        public static string GetOSName()
        {
            try
            {
                string productName = GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
                string csdVersion = GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
                return productName != ""
                    ? (productName.StartsWith("Microsoft") ? "" : "Microsoft ") + $"{productName} {csdVersion}".Trim() + $" | {Environment.OSVersion.VersionString}"
                    : Environment.OSVersion.VersionString;
            }
            catch
            {
                return Environment.OSVersion.VersionString;
            }
        }


        /// <summary>
        /// Url to error logging.
        /// </summary>
        private static string ErrorUrl => Global.SiteUrl + "error";
    }
}
