﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelSalesAnalyzer
{
    public static class SalesTrendAnalyzer
    {
        public static async Task RunAsync()
        {
            var kernel = KernelProvider.GetKernel();
            string csvPath = SalesDataUtilities.GetCsvPath();

            if (!SalesDataUtilities.CheckCsvExists(csvPath))
            {
                return;
            }

            var salesData = SalesDataUtilities.LoadSalesData(csvPath);
            var analysis = await AnalyzeSalesTrendsAsync(kernel, salesData);
            Console.WriteLine("Sales Trends Analysis:\n" + analysis);
        }

        private static async Task<string> AnalyzeSalesTrendsAsync(Kernel kernel, List<SalesRecord> salesData)
        {
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // Prepare data summary for AI
            var dataSummary = string.Join(", ", salesData.Select(r => $"{r.Date}: {r.Sales:C}"));
            var totalSales = salesData.Sum(r => r.Sales);
            var averageSales = salesData.Average(r => r.Sales);
            var maxSales = salesData.Max(r => r.Sales);
            var minSales = salesData.Min(r => r.Sales);

            var chat = new ChatHistory();
            chat.AddSystemMessage("You are a professional sales analyst. Analyze sales data, identify trends, calculate metrics, and provide insights clearly and concisely.");
            chat.AddUserMessage($"Analyze this sales data: [{dataSummary}]. Total sales: {totalSales:C}, Average: {averageSales:C}, Max: {maxSales:C}, Min: {minSales:C}. Identify trends, peak periods, and provide business insights.");

            var settings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.3,
                MaxTokens = 1000
            };

            var response = await chatService.GetChatMessageContentAsync(chat, executionSettings: settings);
            return response.Content ?? string.Empty;
        }
    }
}
