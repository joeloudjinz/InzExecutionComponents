using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(
    name: ExecutionEventKeys.PerformanceMetricsUpdate,
    requiredStoreKeys: [ContextStoreKeys.RiskMetrics]
)]
public class PerformanceMetricsUpdateEvent : IExecutionEvent
{
    public Task PerformEventTask(IExecutionPlanContext context)
    {
        var executionMetrics = context.Store.Get<RiskMetrics>(ContextStoreKeys.RiskMetrics);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Updating performance metrics");

        // Update performance metrics based on execution results
        UpdatePerformanceMetrics(executionMetrics);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Performance metrics updated:");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Portfolio Value: ${executionMetrics.PortfolioValue:F2}");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Current VaR: ${executionMetrics.VaR:F2}");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Sharpe Ratio: {executionMetrics.SharpeRatio:F2}");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Max Drawdown: {(executionMetrics.MaxDrawdown * 100):F2}%");
        return Task.CompletedTask;

        // In a real system, this would store metrics in a time-series database
        // for historical analysis and reporting

        // await Task.Delay(100); // Simulate metrics update processing time
    }

    private void UpdatePerformanceMetrics(RiskMetrics metrics)
    {
        // In a real system, this would update metrics based on actual performance
        // For this example, we'll just log that the update occurred

        // The metrics have already been calculated in the execution plan
        // This event is responsible for storing/persisting them
    }
}