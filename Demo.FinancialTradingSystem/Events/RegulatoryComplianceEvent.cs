using Demo.FinancialTradingSystem.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

namespace Demo.FinancialTradingSystem.Events;

[ServiceExecutionEvent(ExecutionEventKeys.RegulatoryCompliance)]
public class RegulatoryComplianceEvent : IExecutionEvent
{
    public Task PerformEventTask(IExecutionContext context)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Performing regulatory compliance check");
        
        // Simulate regulatory compliance check
        // await Task.Delay(300); // Simulate processing time for compliance checks
        
        // In a real system, this would check against regulatory requirements
        var isCompliant = CheckCompliance(context);
        
        if (!isCompliant)
        {
            throw new InvalidOperationException("Trade does not comply with regulatory requirements");
        }
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {GetType().Name} - Regulatory compliance check passed");
        return Task.CompletedTask;
    }

    private bool CheckCompliance(IExecutionContext context)
    {
        // This is a simplified compliance check
        // In a real system, this would validate against multiple regulatory rules
        // such as MiFID II, SEC regulations, etc.
        
        // For this example, we'll just return true
        // In reality, this would check against rules like:
        // - Is the asset in restricted list?
        // - Are we exceeding position limits?
        // - Is this during a blackout window?
        // - Are required approvals present?
        
        return true;
    }
}