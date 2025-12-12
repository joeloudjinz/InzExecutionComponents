using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionPlan;

namespace Demo.FinancialTradingSystem.ExecutionPlans;

public record TradingInputData : IExecutionParametersContract
{
    [ExecutionContextStoreKey(ContextStoreKeys.MarketData)]
    public MarketData MarketData { get; set; } = new();

    [ExecutionContextStoreKey(ContextStoreKeys.CurrentPositions)]
    public PortfolioPosition[] PortfolioPositions { get; set; } = [];

    [ExecutionContextStoreKey(ContextStoreKeys.NewTrade)]
    public TradingSignal TradingSignal { get; set; } = new();

    public decimal RiskThreshold { get; set; }
    public string AssetClass { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
}

public record TradingOutputData : IExecutionResultContract
{
    [ExecutionContextStoreKey(ContextStoreKeys.TradeResults)]
    public TradeExecutionResult[] TradeResults { get; set; } = [];

    [ExecutionContextStoreKey(ContextStoreKeys.UpdatedPositions)]
    public PortfolioPosition[] UpdatedPositions { get; set; } = [];

    [ExecutionContextStoreKey(ContextStoreKeys.RiskMetrics)]
    public RiskMetrics RiskMetrics { get; set; } = new();

    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public record MarketData
{
    public string Symbol { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal BidPrice { get; set; }
    public decimal AskPrice { get; set; }
    public long Volume { get; set; }

    public DateTime Timestamp { get; set; }
    // public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public record PortfolioPosition
{
    public string Asset { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal UnrealizedPnl { get; set; }
    public DateTime LastUpdated { get; set; }
}

public record TradingSignal
{
    public string SignalType { get; set; } = string.Empty; // BUY, SELL, HOLD
    public string Symbol { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal StopLoss { get; set; }
    public decimal TakeProfit { get; set; }
    public DateTime ExpirationTime { get; set; }
}

public record TradeExecutionResult
{
    public string TradeId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty; // BUY or SELL
    public int Quantity { get; set; }
    public decimal ExecutedPrice { get; set; }
    public decimal Commission { get; set; }
    public DateTime ExecutionTime { get; set; }
    public string Status { get; set; } = string.Empty; // SUCCESS, FAILED, PARTIAL
}

public record RiskMetrics
{
    public decimal PortfolioValue { get; set; }
    public decimal Exposure { get; set; }
    public decimal VaR { get; set; } // Value at Risk
    public decimal SharpeRatio { get; set; }
    public decimal MaxDrawdown { get; set; }
    public Dictionary<string, decimal> AssetExposure { get; set; } = new();
}