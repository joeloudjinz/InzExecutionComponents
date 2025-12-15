using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionNotification;

namespace Demo.FinancialTradingSystem.ExecutionNotifications.Handlers;

[ExecutionNotificationHandler(ExecutionNotificationKeys.TradeExecution, ExecutionNotificationHandlerKeys.TradeExecution.MonitoringDashboard)]
public class TradeExecutionMonitoringDashboardHandler : IExecutionNotificationHandler
{
    public async Task Handle(IExecutionEventContext context)
    {
        // Get notification data from context if available
        // In a real implementation, the notification would be passed to the handler
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Sending trade execution notification to monitoring dashboard");
        
        // Simulate sending to monitoring dashboard
        await Task.Delay(50);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Trade execution notification sent to monitoring dashboard");
    }
}

[ExecutionNotificationHandler(ExecutionNotificationKeys.TradeExecution, ExecutionNotificationHandlerKeys.TradeExecution.ComplianceTeam)]
public class TradeExecutionComplianceTeamHandler : IExecutionNotificationHandler
{
    public async Task Handle(IExecutionEventContext context)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Sending trade execution notification to compliance team");
        
        // Simulate sending to compliance team
        await Task.Delay(75);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Trade execution notification sent to compliance team for review");
    }
}

[ExecutionNotificationHandler(ExecutionNotificationKeys.RiskAlert, ExecutionNotificationHandlerKeys.RiskAlert.RiskManager)]
public class RiskAlertRiskManagerHandler : IExecutionNotificationHandler
{
    public async Task Handle(IExecutionEventContext context)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Sending risk alert to risk manager");
        
        // Simulate sending to risk manager
        await Task.Delay(60);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Risk alert sent to risk manager for immediate attention");
    }
}

[ExecutionNotificationHandler(ExecutionNotificationKeys.RiskAlert, ExecutionNotificationHandlerKeys.RiskAlert.SystemAdministrator)]
public class RiskAlertSystemAdministratorHandler : IExecutionNotificationHandler
{
    public async Task Handle(IExecutionEventContext context)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Sending risk alert to system administrator");
        
        // Simulate sending to system administrator
        await Task.Delay(60);
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Risk alert sent to system administrator for system monitoring");
    }
}