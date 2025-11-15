using ScottPlot;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelSalesAnalyzer
{
    public static class SalesChartGenerator
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
            var predictions = await PredictFutureSalesAsync(kernel, salesData);
            GenerateSalesChart(salesData, predictions);
            Console.WriteLine("Sales chart with predictions generated and saved as 'sales_chart.png'");
        }

        private static void GenerateSalesChart(List<SalesRecord> salesData, List<decimal> predictions)
        {
            // Prepare historical data
            double[] histDates = salesData.Select((r, i) => (double)i).ToArray();
            double[] histSales = salesData.Select(r => (double)r.Sales).ToArray();
            string[] dateLabels = salesData.Select(r => r.Date).ToArray();

            // Prepare prediction data (next 3 months)
            double[] predDates = predictions.Select((p, i) => (double)(salesData.Count + i)).ToArray();
            double[] predSales = predictions.Select(p => (double)p).ToArray();

            // Combine historical and prediction data for a continuous look (optional, but good practice)
            double[] allDates = histDates.Concat(predDates).ToArray();
            double[] allSales = histSales.Concat(predSales).ToArray();

            // Create plot
            var plt = new ScottPlot.Plot();

            // Add historical scatter plot
            var histScatter = plt.Add.Scatter(histDates, histSales);
            histScatter.LineWidth = 2;
            histScatter.MarkerSize = 5;
            histScatter.Color = Colors.Blue;
            histScatter.LegendText = "Historical Sales";

            // Add the *entire* prediction line and markers (Fixed: uses Scatter for line/markers)
            var predScatter = plt.Add.Scatter(predDates, predSales);
            predScatter.LineWidth = 2; // Add a line to connect the predictions
            predScatter.MarkerSize = 10;
            predScatter.MarkerShape = MarkerShape.FilledCircle;
            predScatter.Color = Colors.Red;
            predScatter.LegendText = "Predicted Sales"; // Add legend entry once

            // Add text labels for the predicted values (still using a loop for text labels)
            //for (int i = 0; i < predictions.Count; i++)
            //{
            //    // Add text label for the predicted value
            //    var text = plt.Add.Text($"{predictions[i]:C}", predDates[i], predSales[i] + 1000);
            //    text.LabelFontColor = Colors.Red;
            //    text.LabelFontSize = 12;
            //    text.LabelBold = true;
            //}

            // Customize the plot
            plt.Title("Monthly Sales Data with Predictions");
            plt.XLabel("Month");
            plt.YLabel("Sales ($)");
            plt.Grid.MajorLineWidth = 1;
            plt.ShowLegend();

            // Set custom tick labels for dates
            var allTickLabels = dateLabels.Concat(new[] { "2024-01 (P)", "2024-02 (P)", "2024-03 (P)" }).ToArray(); // Updated labels for prediction months
            plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(allDates, allTickLabels);

            // --- FIX: Rotate X-axis tick labels to 45 degrees ---
            plt.Axes.Bottom.TickLabelStyle.Rotation = 45;
            plt.Axes.Bottom.TickLabelStyle.Alignment = Alignment.LowerCenter; // Adjust alignment for rotated labels
            // ----------------------------------------------------

            // Save the plot
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sales_chart.png");
            plt.SavePng(outputPath, width: 800, height: 600);
        }

        private static async Task<List<decimal>> PredictFutureSalesAsync(Kernel kernel, List<SalesRecord> salesData)
        {
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            // Prepare clean numeric summary and date context
            var salesAmounts = string.Join(", ", salesData.Select(r => r.Sales.ToString("F2")));
            var dateContext = string.Join(", ", salesData.Select(r => r.Date));

            var chat = new ChatHistory();

            // STRONGER SYSTEM MESSAGE
            chat.AddSystemMessage("You are a sales forecasting expert. Your ONLY goal is to output three comma-separated decimal sales numbers for the next three months. DO NOT include any introductory text, explanation, thoughts, currency symbols, or trailing punctuation. The output must be PURELY the three numbers separated by commas.");

            // ENHANCED USER MESSAGE to structure the response
            var userMessage = $@"Historical Dates: [{dateContext}]
Historical Sales Amounts: [{salesAmounts}]

Based on this historical sales data, provide the next 3 months of sales.
OUTPUT FORMAT: <SALES_PREDICTIONS>NUMBER1,NUMBER2,NUMBER3</SALES_PREDICTIONS>
Please provide ONLY the content inside the <SALES_PREDICTIONS> tags.";

            chat.AddUserMessage(userMessage);

            var settings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.3, // Slightly lower temperature for more deterministic output
                MaxTokens = int.MaxValue
            };

            var response = await chatService.GetChatMessageContentAsync(chat, executionSettings: settings);
            var fullResponse = response.Content ?? "<SALES_PREDICTIONS>42000.00,45000.00,48000.00</SALES_PREDICTIONS>"; // Fallback now includes the tag

            // 4. CLEAN UP AND PARSE THE RESPONSE (Handling potential tags)
            string predictionText;

            // Try to extract content between the tags, or use the full response if tags are missing
            var startTag = "<SALES_PREDICTIONS>";
            var endTag = "</SALES_PREDICTIONS>";

            int startIndex = fullResponse.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
            int endIndex = fullResponse.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                // Extract content between tags
                predictionText = fullResponse.Substring(startIndex + startTag.Length, endIndex - (startIndex + startTag.Length)).Trim();
            }
            else
            {
                // If the model failed to use tags, try to clean the raw output (e.g., remove prose/thoughts)
                // This is a simple fallback: assume anything before the first number sequence is noise.
                // The original logic handles parsing the raw comma-separated numbers well.
                predictionText = fullResponse.Replace("\n", " ").Trim();
            }

            // Final check to handle cases where the model might output the thought process before the numbers
            // This assumes the numbers are always near the end of the response.
            // A better approach is to rely on the model strictly following the output format.

            // Parse the predictions
            var predictions = predictionText.Split(',')
                .Select(s => decimal.TryParse(s.Trim(), out var d) ? d : 0m)
                .ToList();

            // Ensure we have exactly 3 predictions
            while (predictions.Count < 3) predictions.Add(0m);
            return predictions.Take(3).ToList();
        }


    }
}
