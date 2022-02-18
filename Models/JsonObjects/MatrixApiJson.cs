using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectionsInformation.Models.JsonObjects
{
    /// <summary>
    /// Класс сериализации JSON запросов для DistanceMatrixAPI
    /// </summary>
    class MatrixApiJson
    {
        public string[] destination_addresses { get; set; }
        public string[] origin_addresses { get; set; }
        public Row[] rows { get; set; }
        public string status { get; set; }

        public class Row
        {
            public Element[] elements { get; set; }
        }

        public class Element
        {
            public Distance distance { get; set; }
            public Duration duration { get; set; }
            public Duration_In_Traffic duration_in_traffic { get; set; }
            public string status { get; set; }
        }

        public class Distance
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class Duration
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class Duration_In_Traffic
        {
            public string text { get; set; }
            public int value { get; set; }
        }

    }
}
