using Newtonsoft.Json;
using System;
using System.IO;
using Updater.Common;

namespace Updater.Config
{
    public class Settings
    {
        private static string AppConfigFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Global.SERVER_NAME);
        public static string AppConfigFile => Path.Combine(AppConfigFolder, "updater.json");

        public static Settings Default
        {
            get
            {
                if (_default == null)
                    Load();
                return _default;
            }
        }
        private static Settings _default;

        public static void Load()
        {
            try
            {
                string fileContent = File.ReadAllText(AppConfigFile);
                _default = JsonConvert.DeserializeObject<Settings>(fileContent);
            }
            catch
            {
                _default = new Settings();
            }
        }

        public void Save()
        {
            try
            {
                string fileContent = JsonConvert.SerializeObject(this, Formatting.Indented);
                if (!Directory.Exists(AppConfigFolder))
                    Directory.CreateDirectory(AppConfigFolder);
                File.WriteAllText(AppConfigFile, fileContent);
            }
            catch
            {
                // ignored
            }
        }

        public bool EffectsOff { get; set; }
    }
}
