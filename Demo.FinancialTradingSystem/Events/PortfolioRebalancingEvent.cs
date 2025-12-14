using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(ExecutionEventKeys.PortfolioRebalancing, requiredStoreKeys: [ContextStoreKeys.TradeResults, ContextStoreKeys.CurrentPositions])]
public class PortfolioRebalancingEvent : IExecutionEvent
{
    public Task PerformEventTask(IExecutionPlanContext context)
    {
        var tradeResults = context.Store.Get<TradingOutputData>(ContextStoreKeys.TradeResults);
        var currentPositions = context.Store.Get<PortfolioPosition[]>(ContextStoreKeys.CurrentPositions);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Starting portfolio rebalancing");
        
        // Update portfolio positions based on executed trades
        var updatedPositions = RebalancePortfolio(currentPositions, tradeResults.TradeResults);
        
        // Store updated positions back to context
        context.Store.Set(ContextStoreKeys.CurrentPositions, updatedPositions);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Portfolio rebalancing completed. Updated {updatedPositions.Length} positions.");
        
        foreach (var position in updatedPositions)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Position: {position.Asset} | Quantity: {position.Quantity} | Value: ${position.CurrentValue:F2}");
        }

        return Task.CompletedTask;

        // await Task.Delay(150); // Simulate rebalancing processing time
    }

    private PortfolioPosition[] RebalancePortfolio(PortfolioPosition[] currentPositions, TradeExecutionResult[] tradeResults)
    {
        var positionsDict = currentPositions.ToDictionary(p => p.Asset, p => p);
        
        // Process each trade to update positions
        foreach (var trade in tradeResults)
        {
            if (positionsDict.ContainsKey(trade.Symbol))
            {
                var position = positionsDict[trade.Symbol];
                
                // Update quantity based on trade
                if (trade.Side.Equals("BUY", StringComparison.OrdinalIgnoreCase))
                {
                    position.Quantity += trade.Quantity;
                }
                else if (trade.Side.Equals("SELL", StringComparison.OrdinalIgnoreCase))
                {
                    position.Quantity -= trade.Quantity;
                }
                
                // Update position value at executed price
                position.CurrentValue = position.Quantity * trade.ExecutedPrice;
                position.LastUpdated = DateTime.UtcNow;
                
                // Recalculate average cost if needed
                // (Simplified - in reality would use more complex methods like FIFO/LIFO)
            }
            else
            {
                // New position if it doesn't exist
                positionsDict[trade.Symbol] = new PortfolioPosition
                {
                    Asset = trade.Symbol,
                    Quantity = trade.Side.Equals("BUY", StringComparison.OrdinalIgnoreCase) ? trade.Quantity : -trade.Quantity,
                    AverageCost = trade.ExecutedPrice,
                    CurrentValue = trade.Quantity * trade.ExecutedPrice,
                    LastUpdated = DateTime.UtcNow
                };
            }
        }
        
        // Remove positions with zero quantity
        var result = positionsDict.Values.Where(p => p.Quantity != 0).ToArray();
        
        return result;
    }
}