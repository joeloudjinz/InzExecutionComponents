using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(ExecutionEventKeys.TradeLogging, requiredStoreKeys: [ContextStoreKeys.TradeResults])]
public class TradeLoggingEvent : IExecutionEvent
{
    public Task PerformEventTask(IExecutionPlanContext context)
    {
        var tradeResults = context.Store.Get<TradeExecutionResult[]>(ContextStoreKeys.TradeResults);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Logging trade execution results");
        
        // Log each trade result to audit trail
        foreach (var trade in tradeResults)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Trade Logged: {trade.TradeId} | {trade.Symbol} | {trade.Side} | Qty: {trade.Quantity} | Price: ${trade.ExecutedPrice:F2} | Status: {trade.Status}");
            
            // In a real system, this would write to a database or audit log
            // For this example, we'll just simulate the logging process
        }
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Successfully logged {tradeResults.Length} trades to audit trail");
        return Task.CompletedTask;

        // await Task.Delay(100); // Simulate logging processing time
    }
}