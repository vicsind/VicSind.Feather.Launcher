using System;
using System.IO;
using System.Text;
using Updater.Config;

namespace Updater.SAH
{
    public class SahData
    {
        /// <summary>
        /// Root folder of SAH.
        /// </summary>
        public SahFolder RootFolder { get; private set; }

        /// <summary>
        /// Path to SAH file.
        /// </summary>
        public string SahPath { get; }

        /// <summary>
        /// Create instance of SahData/
        /// </summary>
        /// <param name="path">Path to SAH file.</param>
        public SahData(string path)
        {
            SahPath = path;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            try
            {
                // Чтение с шифрованием
                Load(XorKey);
            }
            catch
            {
                Load(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        #region Load

        private void Load(byte xorKey)
        {
            RootFolder = new SahFolder("Data", null);
            var bytes = File.ReadAllBytes(SahPath);
            using (var stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadBytes(56);
                    ReadFiles(reader, RootFolder, xorKey);
                }
            }
        }
        #endregion

        /// <summary>
        /// Считывание файлов
        /// </summary>
        #region ReadFiles

        private static void ReadFiles(BinaryReader reader, SahFolder currentFolder, byte xorKey)
        {
            // Количество файлов в данном каталоге
            int count = reader.ReadInt32() ^ xorKey;
            for (int i = 0; i < count; i++)
            {
                string name = Encoding.Default.GetString(reader.ReadBytes(reader.ReadInt32())).Trim('\0');
                long offset = reader.ReadInt64(); // Смещение. Стартовая позиция в saf
                long size = reader.ReadInt64(); // Размер файла

                SahFile newFile = new SahFile(name, offset, size, currentFolder);
                currentFolder.Files.Add(newFile);
            }

            ReadFolders(reader, currentFolder, xorKey);
        }
        #endregion

        /// <summary>
        /// Считывание каталогов
        /// </summary>
        #region ReadFolders

        private static void ReadFolders(BinaryReader reader, SahFolder currentFolder, byte xorKey)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = Encoding.Default.GetString(reader.ReadBytes(reader.ReadInt32())).Trim('\0');

                SahFolder folder = new SahFolder(name, currentFolder);
                currentFolder.Folders.Add(folder);

                // Получение списка файлов из данной директории
                ReadFiles(reader, folder, xorKey);
            }
        }
        #endregion


        /// <summary>
        /// Patch files from folder to current data.
        /// </summary>
        /// <param name="safStream">Stream of SAF data file.</param>
        /// <param name="directoryInfo">Patch directory info.</param>
        public void Patch(FileStream safStream, DirectoryInfo directoryInfo)
        {
            PatchFiles(safStream, directoryInfo, RootFolder);
        }

        /// <summary>
        /// Patch folder to current SAH folder.
        /// </summary>
        #region PatchFolder

        private static void PatchFolder(FileStream safStream, DirectoryInfo directoryInfo, SahFolder parent)
        {
            foreach (DirectoryInfo patchDir in directoryInfo.GetDirectories())
            {
                SahFolder newFolder = new SahFolder(patchDir.Name, parent);
                SahFolder oldFolder = parent.Folders.Find(x => string.Compare(x.Name, patchDir.Name, StringComparison.OrdinalIgnoreCase) == 0);
                if (oldFolder == null)
                {
                    parent.Folders.Add(newFolder);
                }

                // Обновление файлов из папки
                PatchFiles(safStream, patchDir, oldFolder ?? newFolder);
            }
        }
        #endregion

        /// <summary>
        /// Patch files from folder.
        /// </summary>
        #region PatchFiles

        private static void PatchFiles(FileStream safStream, DirectoryInfo directoryInfo, SahFolder parent)
        {
            // All files in patch directory.
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                using (FileStream fileStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // Read patch file bytes.
                    byte[] fileBytes = new byte[fileStream.Length];
                    fileStream.Read(fileBytes, 0, fileBytes.Length);

                    SahFile newFile = new SahFile(fileInfo.Name, fileInfo.Length, parent);
                    SahFile oldFile = parent.Files.Find(x => string.Compare(x.Name, fileInfo.Name, StringComparison.OrdinalIgnoreCase) == 0);

                    // Replace current in SAF.
                    if (oldFile != null && oldFile.Size >= newFile.Size)
                    {
                        newFile.Offset = oldFile.Offset;
                        safStream.Seek(oldFile.Offset, SeekOrigin.Begin);
                        safStream.Write(fileBytes, 0, fileBytes.Length);
                    }
                    // Write file to SAF end.
                    else
                    {
                        newFile.Offset = safStream.Length;
                        safStream.Seek(0, SeekOrigin.End);
                        safStream.Write(fileBytes, 0, fileBytes.Length);
                    }

                    parent.Files.Remove(oldFile);
                    parent.Files.Add(newFile);
                }
            }

            PatchFolder(safStream, directoryInfo, parent);
        }
        #endregion


        /// <summary>
        /// Save SAH by current path.
        /// </summary>
        public void Save()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(SahPath, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                writer.Write(Encoding.Default.GetBytes("SAH"));
                writer.Write(0);
                writer.Write(0); // Filecount
                writer.Write(new byte[40]);
                writer.Write((byte)1);
                writer.Write(0);

                WriteFiles(writer, RootFolder);
                writer.Write(0);
            }
        }

        /// <summary>
        /// Write folder files info to writer stream.
        /// </summary>
        #region WriteFiles

        private static void WriteFiles(BinaryWriter writer, SahFolder folder)
        {
            int filesCount = folder.Files.Count ^ XorKey;
            writer.Write(filesCount);
            foreach (SahFile file in folder.Files)
            {
                string name = GetFileName(file.Name);
                byte[] nameBytes = Encoding.Default.GetBytes(name);
                writer.Write(nameBytes.Length + 1);
                writer.Write(nameBytes);
                writer.Write((byte)0);
                writer.Write(file.Offset);
                writer.Write(file.Size);
            }

            WriteFolders(writer, folder);
        }
        #endregion

        /// <summary>
        /// Write folders info to writer stream.
        /// </summary>
        #region WriteFolders

        private static void WriteFolders(BinaryWriter writer, SahFolder folder)
        {
            writer.Write(folder.Folders.Count);
            foreach (SahFolder subFolder in folder.Folders)
            {
                string name = subFolder.Name;
                byte[] nameBytes = Encoding.Default.GetBytes(name);
                writer.Write(nameBytes.Length + 1);
                writer.Write(nameBytes);
                writer.Write((byte)0);

                WriteFiles(writer, subFolder);
            }
        }
        #endregion


        /// <summary>
        /// Get filename according "EffectsOff" settings.
        /// </summary>
        /// <returns></returns>
        private static string GetFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".eft" when Settings.Default.EffectsOff:
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.XXX";
                    break;
                case ".xxx" when !Settings.Default.EffectsOff:
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.EFT";
                    break;
            }

            return fileName;
        }

        private static byte XorKey => Convert.ToByte(Properties.Resources.FileCountXorKey);
    }
}
