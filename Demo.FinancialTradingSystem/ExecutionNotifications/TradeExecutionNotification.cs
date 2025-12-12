using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;

namespace Demo.FinancialTradingSystem.ExecutionNotifications;

[ExecutionNotification(ExecutionNotificationKeys.TradeExecution)]
public class TradeExecutionNotification
{
    public string TradeId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal ExecutedPrice { get; set; }
    public DateTime ExecutionTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Commission { get; set; }
    public string ExecutionId { get; set; } = string.Empty;
}

[ExecutionNotification(ExecutionNotificationKeys.RiskAlert)]
public class RiskAlertNotification
{
    public string AlertId { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty; // VaR Exceeded, Position Limit Exceeded, etc.
    public string Asset { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal Threshold { get; set; }
    public decimal ActualValue { get; set; }
    public DateTime AlertTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ExecutionId { get; set; } = string.Empty;
}