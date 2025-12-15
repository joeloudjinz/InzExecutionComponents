using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(
    name: ExecutionEventKeys.LiquidityAssessment,
    requiredStoreKeys: [ContextStoreKeys.MarketData]
)]
public class LiquidityAssessmentEvent : IExecutionEvent
{
    public Task Perform(IExecutionEventContext context)
    {
        var marketData = context.Store.Get<MarketData>(ContextStoreKeys.MarketData);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Assessing liquidity for {marketData.Symbol}");

        // Simulate liquidity assessment
        // await Task.Delay(150); // Simulate processing time

        // Check if the market has sufficient liquidity for the intended trade
        var hasSufficientLiquidity = AssessLiquidity(marketData);

        if (!hasSufficientLiquidity)
        {
            throw new InvalidOperationException($"Insufficient liquidity for trading {marketData.Symbol}. Volume: {marketData.Volume}");
        }

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Sufficient liquidity confirmed for {marketData.Symbol}");
        return Task.CompletedTask;
    }

    private bool AssessLiquidity(MarketData marketData)
    {
        // Simple liquidity check - ensure minimum volume threshold
        const long minVolumeThreshold = 100000;
        return marketData.Volume >= minVolumeThreshold;
    }
}