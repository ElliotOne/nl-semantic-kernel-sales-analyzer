﻿namespace SemanticKernelSalesAnalyzer
{
    public static class SalesDataLoader
    {
        public static async Task RunAsync()
        {
            string csvPath = SalesDataUtilities.GetCsvPath();

            if (!SalesDataUtilities.CheckCsvExists(csvPath))
            {
                return;
            }

            var salesData = SalesDataUtilities.LoadSalesData(csvPath);

            Console.WriteLine("Loaded Sales Data (First 5 records):");
            Console.WriteLine("Date\t\tSales");
            Console.WriteLine("----\t\t-----");

            foreach (var record in salesData.Take(5))
            {
                Console.WriteLine($"{record.Date}\t{record.Sales:C}");
            }

            Console.WriteLine($"\nTotal records loaded: {salesData.Count}");
        }
    }
}
