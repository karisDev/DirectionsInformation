using DirectionsInformation.Infrastructure.Commands;
using DirectionsInformation.Models;
using DirectionsInformation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Data;

namespace DirectionsInformation.ViewModels
{
    class DirectionWindowViewModel : ViewModel
    {

        const int AMOUNT_OF_TIMERS = 3; // количество таймеров

        TimedExecution timers;
        #region Переменные
        #region Заголовок окна
        private string _Title = "Информация о точках";
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion

        #region Время последнего запроса
        private string _LastRequestTime = "(давным давно)";
        public string LastRequestTime
        {
            get => _LastRequestTime;
            set => Set(ref _LastRequestTime, value);
        }
        #endregion

        #region Адреса
        private string _Locations = string.Join("\n", AppConstants.Points);
        public string Locations
        {
            get => _Locations;
            set => Set(ref _Locations, value);
        }
        #endregion

        #region Таблица
        DataView _MatrixTable = new DataView();
        public DataView MatrixTable
        {
            get => _MatrixTable;
            set => Set(ref _MatrixTable, value);
        }
        #endregion

        #region TimeChecked
        private bool[] _CheckedTimers = new bool[AMOUNT_OF_TIMERS] { true, true, true };
        #endregion

        #region TimeString
        private string[] _TimeString = new string[AMOUNT_OF_TIMERS];

        public string TimeString1
        {
            get => _TimeString[0];
            set => Set(ref _TimeString[0], value);
        }
        public string TimeString2
        {
            get => _TimeString[1];
            set => Set(ref _TimeString[1], value);
        }
        public string TimeString3
        {
            get => _TimeString[2];
            set => Set(ref _TimeString[2], value);
        }
        #endregion

        #region Статус запроса
        private bool _RequestInProgress = false;
        public bool RequestInProgress
        {
            get => _RequestInProgress;
            set => Set(ref _RequestInProgress, value);
        }
        #endregion

        #endregion

        #region Команды

        #region SaveTimersCommand
        public ICommand SaveTimersCommand { get; }
        private void OnSaveTimersCommandExecuted(object p)
        {
            if (!ValidateTimeFormat())
            {
                MessageBox.Show("Неправильно выставлено время");
                return;
            }
            timers.UpdateTimers(GetCheckedTimersList());
            timers.Start();
        }
        #endregion

        #region CloseAppCommand
        public ICommand CloseAppCommand { get; }
        private void OnCloseAppCommandExecuted(object p)
        {
            SaveSettings();
            Application.Current.Shutdown();
        }
        #endregion

        #region MakeRequestCommand
        public ICommand MakeRequestCommand { get; }
        private void OnMakeRequestCommandExecuted(object p)
        {
            MakeNewRequest();
        }
        #endregion

        #region OpenFolderCommand
        public ICommand OpenFolderCommand { get; }
        private void OnOpenFolderCommandExecuted(object p)
        {
            try
            {
                var systemPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Process.Start(Path.Combine(systemPath, "DirectionsInformation") + "\\"); // открываем папку с логами
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Чтобы посмотреть логи, сначала их нужно создать");
            }
        }
        #endregion

        #endregion

        #region Функции
        private async void MakeNewRequest()
        {
            // проверка запроса на правильность
            if (string.IsNullOrWhiteSpace(_Locations))
            {
                MessageBox.Show("Неверно указаны точки.");
                return;
            }
            RequestInProgress = true;

            // создание и отправка запроса
            MatrixApiHelper matrixApiHelper = new MatrixApiHelper(_Locations.Replace("\r", "").Split('\n'));
            try
            {
                await matrixApiHelper.ExecuteQuerry();
            }
            catch
            {
                RequestInProgress = false;
                MessageBox.Show("У программы нет доступа к интернету.");
                return;
            }

            string csvText = matrixApiHelper.QuerryToCsvString();
            if (!CsvHelper.SaveNearExecutable(csvText))
                MessageBox.Show("Ошибка при создании логов в корне программы. Возможно у программы недостаточно прав.");

            // визуализация
            MatrixTable = DataGridHelper.CsvToDataViewConverter(csvText);
            LastRequestTime = DateTime.Now.ToString("HH:mm");
            RequestInProgress = false;
            SendToastedNotification("Получены обновленные данные", "Нажмите, чтобы посмотреть");
        }

        private List<string> GetCheckedTimersList()
        {
            var result = new List<string>();
            for (int i = 0; i < AMOUNT_OF_TIMERS; i++)
            {
                if (_CheckedTimers[i])
                result.Add(_TimeString[i]);
            }

            return result;
        }

        /// <summary>
        /// Проверка даты на правильность
        /// </summary>
        /// <returns></returns>
        private bool ValidateTimeFormat()
        {
            foreach (string dateStr in GetCheckedTimersList())
            {
                try
                {
                    DateTime dateTime = DateTime.ParseExact(dateStr, "H:mm", null, System.Globalization.DateTimeStyles.None);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private readonly Forms.NotifyIcon notifyIcon;
        /// <summary>
        /// Отправка уведомления в интерфейс Windows
        /// </summary>
        private void SendToastedNotification(string Title, string Text)
        {
            notifyIcon.ShowBalloonTip(3000, Title, Text, Forms.ToolTipIcon.Info);
        }

        private void SaveSettings()
        {
            for (int i = 0; i < AMOUNT_OF_TIMERS; i++)
            {
                AppConstants.Timers[i] = _TimeString[i];
            }    
            AppConstants.Points = Locations.Split('\n').ToList();

            AppConstants.Save();
        }
        #endregion

        public DirectionWindowViewModel(Forms.NotifyIcon _notifyIcon)
        {
            SaveTimersCommand = new LambdaCommand(OnSaveTimersCommandExecuted);
            CloseAppCommand = new LambdaCommand(OnCloseAppCommandExecuted);
            MakeRequestCommand = new LambdaCommand(OnMakeRequestCommandExecuted);
            OpenFolderCommand = new LambdaCommand(OnOpenFolderCommandExecuted);
            notifyIcon = _notifyIcon;
            
            if (AppConstants.Timers == null) return;

            timers = new TimedExecution(AppConstants.Timers, MakeNewRequest);
            timers.Start();

            _TimeString1 = AppConstants.Timers[0];
            _TimeString2 = AppConstants.Timers[1];
            _TimeString3 = AppConstants.Timers[2];
        }
    }
}
