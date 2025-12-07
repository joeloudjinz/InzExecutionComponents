using InzExecutionEvent.ExecutionContext;
using InzExecutionEvent.ExecutionEvent;
using InzExecutionEvent.ExecutionPlan;

namespace InzExecutionEvent.Contracts;

public interface IExecutionComponentManager
{
    public void InitContext();
    public Task LaunchExecution(string label);
}

internal class ExecutionComponentManager : IExecutionComponentManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ExecutionPlanEngine _executionPlanEngine;
    private readonly ExecutionEventEngine _executionEventEngine;

    public ExecutionComponentManager(
        IServiceProvider serviceProvider,
        ExecutionPlanEngine executionPlanEngine,
        ExecutionEventEngine executionEventEngine
    )
    {
        _serviceProvider = serviceProvider;
        _executionPlanEngine = executionPlanEngine;
        _executionEventEngine = executionEventEngine;
        _executionPlanEngine.ServiceProvider = serviceProvider;
    }

    public void InitContext()
    {
        throw new NotImplementedException();
    }

    public async Task LaunchExecution(string label)
    {
        var plan = _executionPlanEngine.RegisteredExecutionPlanContracts.FirstOrDefault(p => p.ExecutionLabel.Equals(label));
        if (plan is null) throw new InvalidOperationException($"Execution plan {label} not found");

        var context = ExecutionContextStaticEngine.Build(_serviceProvider);
        await _executionPlanEngine.PerformExecution(context, plan);
    }
}