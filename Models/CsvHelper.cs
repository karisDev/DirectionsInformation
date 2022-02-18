using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DirectionsInformation.Models
{
    class CsvHelper
    {

        /// <summary>
        /// Сохранение CSV файла в папке "DirectionInformation" рядом с исполняемым файлом
        /// </summary>
        /// <returns>true - успешное сохранение, иначе false</returns>
        public static bool SaveNearExecutable(string csvData)
        {
            try
            {
                var executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var relativeCsvPath = Path.Combine(executablePath, "DirectionsInformation");
                if (!Directory.Exists(relativeCsvPath))
                {
                    Directory.CreateDirectory(relativeCsvPath);
                }
                string fullCsvPath = Path.Combine(relativeCsvPath, DateTime.Now.ToString("Данные от dd-MM-yyyy hh-mm")) + ".csv";

                File.WriteAllText(fullCsvPath, csvData, Encoding.GetEncoding(1251));
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
