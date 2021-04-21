using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using Updater.Logging;

namespace Updater
{
    public static class Helper
    {
        public static string SHA256CheckSum(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return Convert.ToBase64String(sha256.ComputeHash(fileStream));
            }
        }

        public static string CalculateMD5(string filename)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception($"Filename: {filename}. {ex.Message}", ex));
                return "";
            }
        }

        public static bool Checksum(string filename, string checksum)
        {
            return string.Compare(CalculateMD5(filename), checksum, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Закрыть игру
        /// </summary>
        #region ShutdownProcess

        public static bool ShutdownProcess(string name)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(name);
                foreach (Process process in proc)
                    process.Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Удалить файл
        /// </summary>
        /// <param name="filename">Название файла</param>
        /// <param name="tryCount">Количество попыток на удаление файла. Между попытками проходит 0.2с</param>
        /// <returns>True если файл был успешно удален, иначе False</returns>
        #region DeleteFile

        public static bool DeleteFile(string filename, int tryCount = 40)
        {
            // Предпринимаем попытки удаления
            while (--tryCount > 0)
            {
                // Файла не существует
                if (!File.Exists(filename))
                    return true;
                try
                {
                    File.Delete(filename);
                    // Файл удален
                    return true;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
            // Файл не удалось удалить
            return false;
        }
        #endregion

        /// <summary>
        /// Удалить директорию
        /// </summary>
        /// <param name="directoryName">Название файла</param>
        /// <param name="tryCount">Количество попыток на удаление файла. Между попытками проходит 0.2с</param>
        /// <returns>True если файл был успешно удален, иначе False</returns>
        #region DeleteDir

        public static bool DeleteDir(string directoryName, int tryCount = 40)
        {
            // Предпринимаем попытки удаления
            while (--tryCount > 0)
            {
                // Директория не существует
                if (!Directory.Exists(directoryName))
                    return true;

                try
                {
                    Directory.Delete(directoryName, true);
                    // Директория удален
                    return true;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
            // Директорию не удалось удалить
            return false;
        }
        #endregion


        /// <summary>
        /// Запустить апдейтер с указанными аргументами
        /// </summary>
        public static void RunFile(string fileName, string args = "")
        {
            Process process = new Process
            {
                StartInfo = { UseShellExecute = false, FileName = fileName, Arguments = args },
                EnableRaisingEvents = true
            };
            process.Start();
        }
    }
}
