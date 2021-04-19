using System;

namespace Updater.Updating
{
    public interface IUpdatingSystem
    {
        /// <summary>
        /// Start updating.
        /// </summary>
        void Start();

        /// <summary>
        /// Событие, срабатываемое при завершении обновления файлов.
        /// </summary>
        event EventHandler OnCompleted;
    }
}
