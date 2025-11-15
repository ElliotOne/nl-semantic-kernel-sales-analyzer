using CsvHelper;
using System.Globalization;

namespace SemanticKernelSalesAnalyzer
{
    public class SalesRecord
    {
        public string Date { get; set; } = string.Empty;
        public decimal Sales { get; set; }
    }

    public static class SalesDataUtilities
    {
        public static List<SalesRecord> LoadSalesData(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<SalesRecord>().ToList();
        }

        public static string GetCsvPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sales.csv");
        }

        public static bool CheckCsvExists(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                Console.WriteLine("Error: sales.csv file not found in the application directory.");
                return false;
            }
            return true;
        }
    }
}
