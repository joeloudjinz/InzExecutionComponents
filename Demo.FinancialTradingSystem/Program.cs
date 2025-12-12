using Demo.FinancialTradingSystem.ExecutionPlans;
using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents;
using InzExecutionComponents.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add InzExecutionComponents framework
builder.Services.AddInzExecutionComponents(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();

// Use the InzExecutionComponents framework
app.Services.UseInzExecutionComponents();

// Get the execution component manager and launch a trading execution
var executionComponentManager = app.Services.GetRequiredService<IExecutionComponentManager>();

// Create sample trading data for the algorithmic trading execution
var tradingInputData = new TradingInputData
{
    MarketData = new MarketData
    {
        Symbol = "AAPL",
        CurrentPrice = 175.50m,
        BidPrice = 175.45m,
        AskPrice = 175.55m,
        Volume = 5000000,
        Timestamp = DateTime.UtcNow
    },
    PortfolioPositions =
    [
        new PortfolioPosition
        {
            Asset = "AAPL",
            Quantity = 100,
            AverageCost = 165.30m,
            CurrentValue = 17550.00m,
            UnrealizedPnl = 1020.00m,
            LastUpdated = DateTime.UtcNow.AddDays(-1)
        }
    ],
    RiskThreshold = 0.05m, // 5% risk threshold
    TradingSignal = new TradingSignal
    {
        SignalType = "BUY",
        Symbol = "AAPL",
        Quantity = 50,
        TargetPrice = 176.00m,
        StopLoss = 170.00m,
        TakeProfit = 185.00m,
        ExpirationTime = DateTime.UtcNow.AddHours(1)
    },
    AssetClass = "EQUITY",
    Market = "NASDAQ"
};

try
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Starting financial trading system execution...");

    // Launch the algorithmic trading execution
    await executionComponentManager.LaunchExecution(
        label: ExecutionPlanKeys.FinancialTradingAlgo,
        parameters: tradingInputData
    );

    Console.WriteLine();
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] FINANCIAL TRADING SYSTEM EXECUTION COMPLETED SUCCESSFULLY!");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] FINANCIAL TRADING SYSTEM EXECUTION FAILED: {ex.Message}");
    Console.WriteLine(ex);
}

Console.WriteLine();
Console.WriteLine("Done!");