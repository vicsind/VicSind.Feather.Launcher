using System;
using System.Threading.Tasks;
using Updater.Logging;
using Updater.ViewModels;

namespace Updater.Updating
{
    public class UpdatingEngine
    {
        public UpdatingEngine(MainWindowViewModel viewModel, IUpdatingSystem filesUpdatingSystem, IUpdatingSystem dataUpdatingSystem)
        {
            ViewModel = viewModel;
            _filesUpdatingSystem = filesUpdatingSystem;
            _dataUpdatingSystem = dataUpdatingSystem;

            // Updating systems
            _filesUpdatingSystem.OnCompleted += (o, e) => _dataUpdatingSystem.Start();
            _dataUpdatingSystem.OnCompleted += (o, e) => FinishUpdating();
        }
        

        /// <summary>
        /// Начать обновление.
        /// </summary>
        public async void StartUpdating()
        {
            try
            {
                ViewModel.IsStartGameEnabled = false;
                ViewModel.Label3 = "";
                UpdatingInProgress = true;

                await Task.Delay(1000);
                _filesUpdatingSystem.Start();
            }
            catch (Exception ex)
            {
                UpdatingInProgress = false;
                ViewModel.Label1 = Strings.Get(StringType.UpdatingError); // "Ошибка обновления"
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Завершить обновление.
        /// </summary>
        public void FinishUpdating()
        {
            UpdatingInProgress = false;
            ViewModel.Label1 = Strings.Get(StringType.UpdatingCompleted); // "Обновление завершено"
            ViewModel.Label2 = "";
            ViewModel.Label3 = "";
            ViewModel.Progress1 = 100;
            ViewModel.Progress2 = 100;

            ViewModel.IsStartGameEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private MainWindowViewModel ViewModel { get; }

        /// <summary>
        /// Система обновления файлов клиента.
        /// </summary>
        private readonly IUpdatingSystem _filesUpdatingSystem;

        /// <summary>
        /// Система обновления Data файлов.
        /// </summary>
        private readonly IUpdatingSystem _dataUpdatingSystem;


        /// <summary>
        /// Обновление в процессе.
        /// </summary>
        public bool UpdatingInProgress { get; private set; }
    }
}
