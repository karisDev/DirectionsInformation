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
        private bool _TimeChecked1 = true;
        private bool _TimeChecked2 = true;
        private bool _TimeChecked3 = true;
        public bool TimeChecked1
        {
            get => _TimeChecked1;
            set => Set(ref _TimeChecked1, value);
        }
        public bool TimeChecked2
        {
            get => _TimeChecked2;
            set => Set(ref _TimeChecked2, value);
        }
        public bool TimeChecked3
        {
            get => _TimeChecked3;
            set => Set(ref _TimeChecked3, value);
        }
        #endregion

        #region TimeString
        private string _TimeString1;
        private string _TimeString2;
        private string _TimeString3;
        public string TimeString1
        {
            get => _TimeString1;
            set => Set(ref _TimeString1, value);
        }
        public string TimeString2
        {
            get => _TimeString2;
            set => Set(ref _TimeString2, value);
        }
        public string TimeString3
        {
            get => _TimeString3;
            set => Set(ref _TimeString3, value);
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
            MatrixApiHelper matrixApiHelper = new MatrixApiHelper(_Locations.Replace("\r","").Split('\n'));
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

            // сохранение логов
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
            if (_TimeChecked1)
            {
                result.Add(_TimeString1);
            }
            if (_TimeChecked2)
            {
                result.Add(_TimeString2);
            }
            if (_TimeChecked3)
            {
                result.Add(_TimeString3);
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
            AppConstants.Timers[0] = _TimeString1;
            AppConstants.Timers[1] = _TimeString2;
            AppConstants.Timers[2] = _TimeString3;
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

            _TimeString1 = AppConstants.Timers[0];
            _TimeString2 = AppConstants.Timers[1];
            _TimeString3 = AppConstants.Timers[2];

            if (ValidateTimeFormat())
            {
                timers = new TimedExecution(GetCheckedTimersList(), MakeNewRequest);
                timers.Start();
            }
        }
    }
}
