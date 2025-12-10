using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.ExecutionContext;
using InzExecutionComponents.ExecutionEvent;
using InzExecutionComponents.ExecutionPlan;

namespace InzExecutionComponents.Contracts;

public interface IExecutionComponentManager
{
    public Task LaunchExecution(string label, IExecutionParametersContract parameters);
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

    public async Task LaunchExecution(string label, IExecutionParametersContract parameters)
    {
        var plan = _executionPlanEngine.RegisteredExecutionPlanContracts.FirstOrDefault(p => p.ExecutionLabel.Equals(label));
        if (plan is null) throw new InvalidOperationException($"Execution plan {label} was not found");

        var context = ExecutionContextStaticEngine.Build(_serviceProvider);
        context.Store.Set(label, parameters);
        await _executionPlanEngine.PerformExecution(context, plan);
    }
}