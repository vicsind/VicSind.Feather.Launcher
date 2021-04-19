using System.Collections.Generic;

namespace Updater
{
    public static class Strings
    {
        public static string Get(StringType index, params object[] variables)
        {
            string message = List[index];
            for (var i = 0; i < variables.Length; i++)
                message = message.Replace($"<{i}>", variables[i].ToString());

            return message;
        }

        public static Dictionary<StringType, string> List => StringsEn;

        private static readonly Dictionary<StringType, string> StringsRu = new Dictionary<StringType, string>
        {
            { StringType.FileNotFound, "file not found" },
            { StringType.CheckingUpdates, "Checking updates"},
            { StringType.NoUpdates, "No updates"},
            { StringType.UpdatingClientFiles, "Updating client files"},
            { StringType.FileNotFoundOnServer, "File <0> not found on server"},
            { StringType.Unpacking, "Unpacking" },
            { StringType.UpdatingLauncher, "Updating launcher" },

            { StringType.Downloading, "Downloading" },
            { StringType.DownloadingCompleted, "Downloading complete"},
            { StringType.DownloadingError, "Downloading error"},

            { StringType.Updating, "Updating" },
            { StringType.UpdatingCompleted, "Updating complete" },
            { StringType.UpdatingError, "Updating error" },

            { StringType.FileReadingError, "File <0> reading error!" },
            { StringType.NoRequiredFilesToStart, "Error! No required files to start" },
            { StringType.Starting, "Starting" },
            { StringType.RequiredToCloseGame, "You must close the game before updating" },
            { StringType.ServerNotAvailable, "Server is not available" },
            { StringType.GameStarted, "Game started" },
            { StringType.ReadyToStart, "Ready to start" },
            { StringType.Today, "Today" },
            { StringType.Yesterday, "Yesterday" },
            { StringType.FoundForbiddenSoftware, "Обнаружено запрещённое ПО" },

            { StringType.ERROR, "ОШИБКА" },
            { StringType.REQUIRED_CLOSE_THE_GAME, "НЕОБХОДИМО ЗАКРЫТЬ ИГРУ ПЕРЕД ЗАПУСКОМ" },
            { StringType.NO_REQUIRED_FILES, "НЕТ НЕОБХОДИМЫХ ФАЙЛОВ ДЛЯ ЗАПУСКА ИГРЫ" },
            { StringType.FILE_MISSING, "Отсутствует файл <0>" },
            { StringType.ERROR_MESSAGE, "Error message: <0>" },
            { StringType.PLEASE_SEND_FILE, "Отправьте, пожалуйста, файл <0> в поддержку для устранения ошибки" },
        };

        private static readonly Dictionary<StringType, string> StringsEn = new Dictionary<StringType, string>
        {
            { StringType.FileNotFound, "file not found" },
            { StringType.CheckingUpdates, "Checking updates"},
            { StringType.NoUpdates, "No updates"},
            { StringType.UpdatingClientFiles, "Updating client files"},
            { StringType.FileNotFoundOnServer, "File <0> not found on server"},
            { StringType.Unpacking, "Unpacking" },
            { StringType.UpdatingLauncher, "Updating launcher" },

            { StringType.Downloading, "Downloading" },
            { StringType.DownloadingCompleted, "Downloading complete"},
            { StringType.DownloadingError, "Downloading error"},

            { StringType.Updating, "Updating" },
            { StringType.UpdatingCompleted, "Updating complete" },
            { StringType.UpdatingError, "Updating error" },

            { StringType.FileReadingError, "File <0> reading error!" },
            { StringType.NoRequiredFilesToStart, "Error! No required files to start" },
            { StringType.Starting, "Starting" },
            { StringType.RequiredToCloseGame, "You must close the game before updating" },
            { StringType.ServerNotAvailable, "Server is not available" },
            { StringType.GameStarted, "Game started" },
            { StringType.ReadyToStart, "Ready to start" },
            { StringType.Today, "Today" },
            { StringType.Yesterday, "Yesterday" },
            { StringType.FoundForbiddenSoftware, "Found forbidden Software" },

            { StringType.ERROR, "ERROR" },
            { StringType.REQUIRED_CLOSE_THE_GAME, "YOU MUST CLOSE THE GAME BEFORE STARTING" },
            { StringType.NO_REQUIRED_FILES, "NO FILES REQUIRED TO START THE GAME" },
            { StringType.FILE_MISSING, "<0> file missing" },
            { StringType.ERROR_MESSAGE, "Error message: <0>" },
            { StringType.PLEASE_SEND_FILE, "Please, send file <0> to our support" },
        };
    }

    public enum StringType
    {
        FileNotFound,
        CheckingUpdates,
        NoUpdates,
        UpdatingClientFiles,
        FileNotFoundOnServer,
        Unpacking,
        UpdatingLauncher,

        Downloading,
        DownloadingCompleted,
        DownloadingError,

        Updating,
        UpdatingCompleted,
        UpdatingError,

        FileReadingError,
        NoRequiredFilesToStart,
        Starting,
        RequiredToCloseGame,
        ServerNotAvailable,
        GameStarted,
        ReadyToStart,
        FoundForbiddenSoftware,

        Today,
        Yesterday,

        ERROR,
        ERROR_MESSAGE,
        REQUIRED_CLOSE_THE_GAME,
        NO_REQUIRED_FILES,
        FILE_MISSING,
        PLEASE_SEND_FILE,
    }
}
