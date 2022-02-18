using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectionsInformation.Models
{
    static class DataGridHelper
    {
        public static string[][] CsvToMatrixConverter(string csvData)
        {
            List<string> Input = csvData.Split('\n').ToList();
            string[][] matrixString = new string[Input.Count()][];
            for (int i = 0; i < Input.Count(); i++)
            {
                matrixString[i] = Input[i].Split(',');
            }
            return matrixString;
        }

        // за подсказку про DataView спасибо https://www.cyberforum.ru/wpf-silverlight/thread2425216.html
        public static DataView MatrixToDataViewConverter(string[][] matrix)
        {
            var result = new DataTable();

            for (int i = 0; i < matrix[0].Length; i++)
            {
                result.Columns.Add("col" + i, typeof(string));
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                var row = result.NewRow();

                for (int j = 0; j < matrix[0].Length; j++)
                {
                    row[j] = matrix[i][j];
                }

                result.Rows.Add(row);
            }

            return result.DefaultView;
        }

        public static DataView CsvToDataViewConverter(string csvData) => MatrixToDataViewConverter(CsvToMatrixConverter(csvData));
    }
}
