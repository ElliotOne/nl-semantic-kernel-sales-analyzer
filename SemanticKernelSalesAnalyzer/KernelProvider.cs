using Microsoft.SemanticKernel;

namespace SemanticKernelSalesAnalyzer
{
    public static class KernelProvider
    {
        private static Kernel? _kernel;

        public static Kernel GetKernel()
        {
            if (_kernel != null) return _kernel;

            var builder = Kernel.CreateBuilder();

            builder.AddOpenAIChatCompletion(
                modelId: "deepseek/deepseek-r1-0528-qwen3-8b",
                apiKey: "not-needed",
                endpoint: new Uri("http://localhost:1234/v1")
            );

            _kernel = builder.Build();
            return _kernel;
        }
    }
}
