using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelSalesAnalyzer
{
    public static class SalesAnomalyDetector
    {
        public static async Task RunAsync()
        {
            string csvPath = SalesDataUtilities.GetCsvPath();

            if (!SalesDataUtilities.CheckCsvExists(csvPath))
            {
                return;
            }

            var salesData = SalesDataUtilities.LoadSalesData(csvPath);
            var kernel = KernelProvider.GetKernel();

            // Calculate basic statistics
            var salesAmounts = salesData.Select(r => r.Sales).ToList();
            var mean = salesAmounts.Average();
            var stdDev = CalculateStandardDeviation(salesAmounts, mean);

            // Detect anomalies using statistical method (e.g., beyond 2 standard deviations)
            var anomalies = DetectAnomalies(salesData, mean, stdDev);

            // Use AI to analyze and explain anomalies
            var analysis = await AnalyzeAnomaliesAsync(kernel, salesData, anomalies, mean, stdDev);

            Console.WriteLine("Sales Anomalies Detection Report:\n" + analysis);
        }

        private static double CalculateStandardDeviation(List<decimal> values, decimal mean)
        {
            var variance = values.Sum(v => Math.Pow((double)(v - mean), 2)) / values.Count;
            return Math.Sqrt(variance);
        }

        private static List<SalesRecord> DetectAnomalies(List<SalesRecord> salesData, decimal mean, double stdDev)
        {
            var threshold = 2.0 * stdDev; // Anomalies beyond 2 standard deviations
            return salesData.Where(r => Math.Abs((double)(r.Sales - mean)) > threshold).ToList();
        }

        private static async Task<string> AnalyzeAnomaliesAsync(Kernel kernel, List<SalesRecord> salesData, List<SalesRecord> anomalies, decimal mean, double stdDev)
        {
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            var dataSummary = string.Join(", ", salesData.Select(r => $"{r.Date}: {r.Sales:C}"));
            var anomalySummary = string.Join(", ", anomalies.Select(r => $"{r.Date}: {r.Sales:C}"));

            var chat = new ChatHistory();
            chat.AddSystemMessage("You are a data analyst specializing in sales anomaly detection. Provide a detailed report on detected anomalies, including possible causes, impacts on business, and recommendations.");
            chat.AddUserMessage($"Sales Data: [{dataSummary}]. Mean Sales: {mean:C}, Standard Deviation: {stdDev:F2}. Detected Anomalies: [{anomalySummary}]. Analyze these anomalies, explain potential reasons, assess business impact, and suggest actions.");

            var settings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.3,
                MaxTokens = int.MaxValue
            };

            var response = await chatService.GetChatMessageContentAsync(chat, executionSettings: settings);
            return response.Content ?? string.Empty;
        }
    }
}
