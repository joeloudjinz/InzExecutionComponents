using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(ExecutionEventKeys.PositionOverlapCheck, requiredStoreKeys: [ContextStoreKeys.CurrentPositions, ContextStoreKeys.NewTrade])]
public class PositionOverlapCheckEvent : IExecutionEvent
{
    public Task PerformEventTask(IExecutionPlanContext context)
    {
        var currentPositions = context.Store.Get<PortfolioPosition[]>(ContextStoreKeys.CurrentPositions);
        var newTrade = context.Store.Get<TradingSignal>(ContextStoreKeys.NewTrade);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Checking for position overlaps");
        
        // Simulate position overlap check
        // await Task.Delay(100); // Simulate processing time
        
        var overlapIssues = CheckPositionOverlap(currentPositions, newTrade);
        
        if (overlapIssues.Any())
        {
            var issues = string.Join(", ", overlapIssues);
            throw new InvalidOperationException($"Position overlap detected: {issues}");
        }
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - No position overlaps found");
        return Task.CompletedTask;
    }

    private List<string> CheckPositionOverlap(PortfolioPosition[] currentPositions, TradingSignal newTrade)
    {
        var issues = new List<string>();
        
        if (string.IsNullOrEmpty(newTrade.Symbol))
        {
            return issues;
        }
        
        // Check if the new trade conflicts with existing positions
        var existingPosition = currentPositions.FirstOrDefault(p => p.Asset == newTrade.Symbol);
        
        if (existingPosition != null)
        {
            // In a real system, this would check for more complex overlaps
            // such as hedging conflicts, concentration limits, etc.
            
            // For this example, we'll just check if the trade exceeds position limits
            var newPositionSize = Math.Abs(newTrade.Quantity * newTrade.TargetPrice);
            var currentPositionSize = Math.Abs(existingPosition.Quantity * existingPosition.AverageCost);
            
            if (newPositionSize > 1000000m) // Example limit
            {
                issues.Add($"Proposed trade would exceed position size limits for {newTrade.Symbol}");
            }
        }
        
        return issues;
    }
}