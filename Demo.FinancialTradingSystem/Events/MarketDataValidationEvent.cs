using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(
    name: ExecutionEventKeys.MarketDataValidation,
    requiredStoreKeys: [ContextStoreKeys.MarketData]
    // requiredConfigurations: [ConfigurationLabels.ValidationRules],
    // inputType: typeof(MarketData),
    // outputType: typeof(ValidationResult)
)]
public class MarketDataValidationEvent : IExecutionEvent
{
    public Task PerformEventTask(IExecutionPlanContext context)
    {
        var marketData = context.Store.Get<MarketData>(ContextStoreKeys.MarketData);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Validating market data for symbol: {marketData.Symbol}");

        // Perform validation checks
        var validationResult = ValidateMarketData(marketData);

        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Market data validation failed: {string.Join(", ", validationResult.Errors)}");
        }

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Market data validation passed for {marketData.Symbol}");
        return Task.CompletedTask;

        // await Task.Delay(200); // Simulate validation processing time
    }

    private ValidationResult ValidateMarketData(MarketData marketData)
    {
        var result = new ValidationResult { IsValid = true, Errors = [] };

        // Check timestamp freshness
        if (DateTime.UtcNow - marketData.Timestamp > TimeSpan.FromSeconds(30))
        {
            result.IsValid = false;
            result.Errors.Add("Market data is too old");
        }

        // Check price validity
        if (marketData.CurrentPrice <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Invalid current price");
        }

        if (marketData.BidPrice <= 0 || marketData.AskPrice <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Invalid bid or ask price");
        }

        if (marketData.BidPrice > marketData.AskPrice)
        {
            result.IsValid = false;
            result.Errors.Add("Bid price higher than ask price");
        }

        // Check for extreme price movements
        var spread = marketData.AskPrice - marketData.BidPrice;
        var spreadPercentage = spread / marketData.CurrentPrice;
        if (spreadPercentage > 0.02m) // More than 2% spread
        {
            result.IsValid = false;
            result.Errors.Add("Unusually wide bid-ask spread detected");
        }

        return result;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
}