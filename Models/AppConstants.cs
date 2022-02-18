using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectionsInformation.Models
{
    /// <summary>
    /// Класс для удобного обращения к файлу Settings.settings
    /// </summary>
    class AppConstants
    {
        public static List<string> Points { get; set; }
        public static List<string> Timers { get; set; }

        public static void Load()
        {
            Points = Properties.Settings.Default.Points.Split(';').ToList<string>();
            Timers = Properties.Settings.Default.Timers.Split(';').ToList<string>();
        }
        
        public static void Save()
        {
            Properties.Settings.Default.Points = string.Join(";", Points);
            Properties.Settings.Default.Timers = string.Join(";", Timers);
            Properties.Settings.Default.Save();
        }
    }
}
