namespace Demo.FinancialTradingSystem.Resources;

public static class ExecutionPlanKeys
{
    public const string FinancialTradingAlgo = "execution-plan.financial.trading-algo";
}

public static class ExecutionEventKeys
{
    public const string MarketDataValidation = "event.service.market-data-validation";
    public const string LiquidityAssessment = "event.service.liquidity-assessment";
    public const string RegulatoryCompliance = "event.service.regulatory-compliance";
    public const string PositionOverlapCheck = "event.service.position-overlap-check";
    public const string TradeLogging = "event.service.trade-logging";
    public const string PortfolioRebalancing = "event.service.portfolio-rebalancing";
    public const string PerformanceMetricsUpdate = "event.service.performance-metrics-update";
}

public static class ExecutionNotificationKeys
{
    public const string TradeExecution = "notification.trade-execution";
    public const string RiskAlert = "notification.risk-alert";
}

public static class ExecutionNotificationHandlerKeys
{
    public static class TradeExecution
    {
        public const string MonitoringDashboard = "notification.handler.trade-execution-monitoring";
        public const string ComplianceTeam = "notification.handler.trade-execution-compliance";
    }

    public static class RiskAlert
    {
        public const string RiskManager = "notification.handler.risk.alert.risk-manager";
        public const string SystemAdministrator = "notification.handler.risk.alert.admin";
    }
}

// public static class ConfigurationLabels
// {
//     public const string TradingRiskLimits = "TradingRiskLimits";
//     public const string ValidationRules = "ValidationRules";
//     public const string ComplianceRules = "ComplianceRules";
// }

public static class ContextStoreKeys
{
    public const string MarketData = "context.store.key.market-data";
    public const string CurrentPositions = "context.store.key.current-positions";
    public const string NewTrade = "context.store.key.new-trade";
    public const string TradeResults = "context.store.key.trade-results";
    public const string UpdatedPositions = "context.store.key.updated-positions";
    public const string RiskMetrics = "context.store.key.resik-metrics";
}

public static class MessagingQueueLabels
{
    public const string TradingAlgoQueue = "messaging.queue.trading-algo";
}