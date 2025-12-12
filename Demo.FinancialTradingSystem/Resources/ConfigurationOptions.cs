namespace Demo.FinancialTradingSystem.Resources;

public class TradingRiskLimitsConfiguration
{
    public decimal MaxPositionSize { get; set; } = 1000000m; // Maximum position size per trade
    public decimal MaxPortfolioExposure { get; set; } = 0.1m; // Maximum 10% exposure to single asset
    public decimal VaRLimit { get; set; } = 0.05m; // Maximum 5% daily VaR
    public decimal StopLossPercentage { get; set; } = 0.05m; // 5% stop loss
    public TimeSpan MaxHoldingPeriod { get; set; } = TimeSpan.FromDays(30); // Max holding period
    public decimal MaxDailyLoss { get; set; } = 0.02m; // 2% max daily loss
}

public class ValidationRulesConfiguration
{
    public int MinPricePrecision { get; set; } = 2;
    public int MaxPriceDeviationPercent { get; set; } = 5; // Max 5% deviation from reference price
    public TimeSpan MaxDataAgeSeconds { get; set; } = TimeSpan.FromSeconds(30); // Data must be recent
    public string[] ValidSymbols { get; set; } = []; // List of valid trading symbols
    public decimal MinOrderSize { get; set; } = 1.0m; // Minimum order size
    public decimal MaxOrderSize { get; set; } = 100000.0m; // Maximum order size
}

public class ComplianceRulesConfiguration
{
    public string[] RestrictedCountries { get; set; } = [];
    public string[] RestrictedAssets { get; set; } = [];
    public TimeSpan[] BlackoutWindows { get; set; } = []; // Times when trading is restricted
    public string[] RequiredApprovals { get; set; } = []; // List of required approvals for certain trades
    public bool EnableRegulatoryReporting { get; set; } = true;
    public string[] ReportRecipients { get; set; } = [];
}