# nl-semantic-kernel-sales-analyzer

A collection of examples demonstrating the capabilities of Microsoft Semantic Kernel for sales data analysis with local AI models via LM Studio. This project showcases various AI-driven tasks such as sales trend analysis, anomaly detection, and predictive charting using a console application built with .NET 8.0.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [LM Studio](https://lmstudio.ai/) installed and running locally
- A compatible model downloaded in LM Studio (e.g., deepseek/deepseek-r1-0528-qwen3-8b)

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/your-username/nl-semantic-kernel-sales-analyzer.git
   cd nl-semantic-kernel-sales-analyzer
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Setup

1. **Install and Start LM Studio:**
   - Download and install LM Studio from [lmstudio.ai](https://lmstudio.ai/)
   - Launch LM Studio and download the required model (deepseek/deepseek-r1-0528-qwen3-8b)
   - Start the local server in LM Studio (typically runs on http://localhost:1234)

2. **Verify Model Configuration:**
   - Ensure the model is loaded and the server is running on the expected endpoint
   - The application is configured to connect to `http://localhost:1234/v1`

## Usage

Run the console application to execute all examples:

```bash
dotnet run --project SemanticKernelSalesAnalyzer
```

The application will run four examples sequentially, demonstrating different AI capabilities for sales analysis.

## Examples

### Example 1: Load Sales Data
Loads sales data from a CSV file and displays the first 5 records along with the total number of records. Demonstrates basic data loading and validation functionality.

### Example 2: Analyze Sales Trends
Uses Semantic Kernel with AI to analyze sales trends from the loaded data. The AI provides insights on trends, peak periods, business metrics, and recommendations based on the historical sales data.

### Example 3: Generate Sales Chart
Creates a visual chart of the sales data using ScottPlot, including AI-generated predictions for the next 3 months. The chart is saved as 'sales_chart.png' and shows both historical and predicted sales trends.

### Example 4: Detect Sales Anomalies
Performs statistical anomaly detection on the sales data (using standard deviation thresholds) and uses AI to analyze and explain any detected anomalies, including potential causes, business impacts, and suggested actions.

## Dependencies

- Microsoft.SemanticKernel v1.67.1
- CsvHelper v33.0.1
- ScottPlot v5.0.36

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
