using System;
using System.Collections.Generic;
using System.Timers;

namespace DirectionsInformation.Models
{
    class TimedExecution
    {
        List<System.Timers.Timer> Timers = new List<Timer>();
        readonly Action TimerReachedAction;

        /// <summary>
        /// Необходимо вручную запустить вызвав Start()
        /// </summary>
        /// <param name="TimeToCheck">Временные промежутки. Пример: { "8:00", "15:00", ...}</param>
        /// <param name="_TimerReachedAction">Функция для вызова по окончанию таймера</param>
        public TimedExecution(List<string> TimeToCheck, Action _TimerReachedAction)
        {
            TimerReachedAction = _TimerReachedAction;
            UpdateTimers(TimeToCheck);
        }

        /// <summary>
        /// Запускает указанную функцию и переводит таймер на 24 часа вперёд
        /// </summary>
        private void TimeReached(object sender, ElapsedEventArgs e)
        {
            Timer t = sender as Timer;
            t.Interval += new TimeSpan(24, 0, 0).TotalMilliseconds;
            TimerReachedAction();
        }

        /// <summary>
        /// Останавливает все таймеры и устанавливает новые. Необходимо вручную запустить вызвав Start()
        /// </summary>
        /// <param name="TimersList">Временные промежутки. Пример: { "8:00", "15:00", ... }</param>
        public void UpdateTimers(List<string> TimersList)
        {
            Stop();
            Timers = new List<Timer>();
            foreach (string time in TimersList)
            {
                string dateStr = time;
                DateTime dateTime;
                
                dateTime = DateTime.ParseExact(dateStr, "H:mm", null, System.Globalization.DateTimeStyles.None);

                if (dateTime < DateTime.Now)
                    dateTime = dateTime.AddDays(1);
                
                Timer t = new Timer((dateTime - DateTime.Now).TotalMilliseconds);
                t.Elapsed += new ElapsedEventHandler(TimeReached);
                Timers.Add(t);
            }
        }

        public void Start()
        {
            foreach (Timer t in Timers)
            {
                t.Start();
            }
        }

        public void Stop()
        {
            foreach (Timer t in Timers)
            {
                t.Stop();
            }
        }
    }
}
