using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Contracts.Models;
using InzExecutionComponents.Extensions;

namespace Demo.FinancialTradingSystem.ExecutionPlans;

[ExecutionPlan(ExecutionPlanKeys.FinancialTradingAlgo)]
[ExecutionInputDataType(typeof(TradingInputData))]
[ExecutionOutputDataType(typeof(TradingOutputData))]
// [ExecutionConfigurationOptions([ConfigurationLabels.TradingRiskLimits])]
[RegisterPreExecutionEvents(ExecutionEventKeys.MarketDataValidation)]
[RegisterPreExecutionEvents(ExecutionEventKeys.LiquidityAssessment, ExecutionEventKeys.RegulatoryCompliance, ExecutionEventKeys.PositionOverlapCheck)]
[RegisterPostExecutionEvents(ExecutionEventKeys.TradeLogging, ExecutionEventKeys.PerformanceMetricsUpdate)]
[PublishExecutionNotifications([ExecutionNotificationKeys.TradeExecution, ExecutionNotificationKeys.RiskAlert])]
public class AlgorithmicTradingExecutionPlan :
    IExecutionContract<IExecutionResultContract>,
    IPreEventsExecutionContract,
    IPostEventsExecutionContract
{
    public Task BeforeDispatchingPreExecutionEvents(IExecutionContext context)
    {
        var marketData = context.Store.Get<MarketData>(ContextStoreKeys.MarketData);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Starting trading execution with ID: {context.ExecutionPlanRegistrationKey}");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Market data validated for symbol: {marketData.Symbol}");
        return Task.CompletedTask;

        // await Task.Delay(100); // Simulate initialization delay
    }

    public Task<ExecutionResult<IExecutionResultContract>> Execute(IExecutionContext context)
    {
        var inputData = context.GetInputData<TradingInputData>();

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Executing trading algorithm for symbol: {inputData.MarketData.Symbol}");

        // Simulate algorithmic trading execution
        var tradeResults = PerformAlgorithmicTrading(inputData);

        var outputData = new TradingOutputData
        {
            TradeResults = tradeResults,
            UpdatedPositions = inputData.PortfolioPositions, // Simplified - in real system would update positions
            RiskMetrics = CalculateRiskMetrics(inputData, tradeResults),
            Success = true,
            Message = "Trade execution completed successfully"
        };

        // Add results to context for post-execution events
        context.SetOutputData(outputData);

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Trading execution completed. Trades executed: {tradeResults.Length}");

        // Add a small delay to make the method truly async
        // await Task.Delay(10);

        return Task.FromResult<ExecutionResult<IExecutionResultContract>>(outputData);
    }

    public Task AfterDispatchingPostExecutionEvents(IExecutionContext context)
    {
        var outputData = context.GetOutputData<TradingOutputData>();

        // Cleanup and finalize execution metrics
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Execution finalized with ID: {context.ExecutionPlanRegistrationKey} - Success: {outputData.Success}");
        return Task.CompletedTask;

        // await Task.Delay(100); // Simulate cleanup delay
    }

    private TradeExecutionResult[] PerformAlgorithmicTrading(TradingInputData inputData)
    {
        // This is a simplified simulation of algorithmic trading
        // In a real system, this would contain complex trading algorithms

        var tradeSignal = inputData.TradingSignal;
        var marketData = inputData.MarketData;

        // Validate trading signal
        if (string.IsNullOrEmpty(tradeSignal.SignalType) || string.IsNullOrEmpty(tradeSignal.Symbol))
        {
            return [];
        }

        // Execute trade based on signal
        var tradeResult = new TradeExecutionResult
        {
            TradeId = $"TRADE_{Guid.NewGuid():N}",
            Symbol = tradeSignal.Symbol,
            Side = tradeSignal.SignalType.ToUpper(),
            Quantity = tradeSignal.Quantity,
            ExecutedPrice = marketData.CurrentPrice,
            Commission = CalculateCommission(tradeSignal.Quantity, marketData.CurrentPrice),
            ExecutionTime = DateTime.UtcNow,
            Status = "SUCCESS"
        };

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Executed trade: {tradeResult.Side} {tradeResult.Quantity} shares of {tradeResult.Symbol} at ${tradeResult.ExecutedPrice:F2}");

        return [tradeResult];
    }

    private decimal CalculateCommission(int quantity, decimal price)
    {
        // Simplified commission calculation
        var tradeValue = quantity * price;
        return Math.Max(1.0m, tradeValue * 0.0001m); // Minimum $1 or 0.01% of trade value
    }

    private RiskMetrics CalculateRiskMetrics(TradingInputData inputData, TradeExecutionResult[] tradeResults)
    {
        // Calculate basic risk metrics
        var totalValue = inputData.PortfolioPositions.Sum(p => p.CurrentValue);
        var totalExposure = tradeResults.Sum(t => t.Quantity * t.ExecutedPrice);

        return new RiskMetrics
        {
            PortfolioValue = totalValue,
            Exposure = totalExposure,
            VaR = totalValue * 0.02m, // Simplified VaR calculation
            SharpeRatio = 1.5m, // Placeholder value
            MaxDrawdown = 0.05m, // 5% max drawdown
            AssetExposure = inputData.PortfolioPositions.ToDictionary(p => p.Asset, p => p.CurrentValue / totalValue)
        };
    }
}