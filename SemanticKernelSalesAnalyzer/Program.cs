﻿using SemanticKernelSalesAnalyzer;

Console.WriteLine("Example 1: Load Sales Data");
await SalesDataLoader.RunAsync();

Console.WriteLine("\nExample 2: Analyze Sales Trends");
await SalesTrendAnalyzer.RunAsync();

Console.WriteLine("\nExample 3: Generate Sales Chart");
await SalesChartGenerator.RunAsync();

Console.WriteLine("\nExample 4: Detect Sales Anomalies");
await SalesAnomalyDetector.RunAsync();
