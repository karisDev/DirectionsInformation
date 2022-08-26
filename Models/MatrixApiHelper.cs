using DirectionsInformation.Models.JsonObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DirectionsInformation.Models
{
    class MatrixApiHelper
    {
        private readonly string[] Locations;
        MatrixApiJson QuerryContent;

        public MatrixApiHelper(string[] locations)
        {
            var temp = new List<string>();
            foreach (string location in locations)
            {
                if (location.Length > 1)
                    temp.Add(location);
            }
            Locations = temp.ToArray();
        }

        public async Task ExecuteQuerry()
        {
            string apiKey = "AIzaSyCtEfXlGJUrgmnD6vA6Y4EM0PbZyA3-_v4"; // вы видите этот ключ не потому что я его случайно слил, а потому что он для меня бесплатный)
            string convertedPoints = ArrayToStringConverter(Locations);

            string url = "https://maps.googleapis.com/maps/api/distancematrix/json" +
                $"?destinations={convertedPoints}" +
                $"&origins={convertedPoints}" +
                $"&key={apiKey}" +
                "&departure_time=now" +
                "&language=ru";

            var client = new HttpClient();
            var result = await client.GetStringAsync(url);
            QuerryContent = JsonConvert.DeserializeObject<MatrixApiJson>(result);
        }

        /// <summary>
        /// Создание строки по правилам CSV
        /// </summary>
        /// <returns></returns>
        public string QuerryToCsvString()
        {
            string csvString = ",";
            string[] destinations = Locations;
            // строка заголовков
            foreach (string destination in destinations)
                csvString += destination.Replace(',', ' ') + ",";
            
            csvString = csvString.Remove(csvString.Length - 1); // запятая в конце не требуется
            
            for (int row = 0; row < destinations.Length; row++)
            {
                csvString += '\n' + destinations[row].Replace(',', ' ') + ",";

                for (int column = 0; column < destinations.Length; column++)
                {
                    if (row == column) // ставим прочерк, если точка отправления совпадает с точкой назначения
                        csvString += "0,";
                    else
                    {
                        // API не вернет значения расстояния или времени, если они слишком большие
                        if (QuerryContent.rows[row].elements[column].distance != null)
                            csvString += QuerryContent.rows[row].elements[column].distance.text.Replace(',', '.') + " / ";
                        else
                            csvString += "более 10 000 км" + " / ";

                        if (QuerryContent.rows[row].elements[column].duration_in_traffic != null)
                            csvString += QuerryContent.rows[row].elements[column].duration_in_traffic.text.Replace(',', '.') + ",";
                        else
                            csvString += "более 2 дн" + ",";
                    }
                }
                csvString = csvString.Remove(csvString.Length - 1);
            }
            return csvString;
        }

        /// <summary>
        /// Создание строки с разделителем `|`, которое в DistanceMatrixAPI обозначает новую локацию
        /// </summary>
        private string ArrayToStringConverter(string[] points)
        {
            string result = points[0];
            for (int i = 1; i < points.Length; i++)
            {
                result += "|" + points[i];
            }
            return result;
        }
    }
}
