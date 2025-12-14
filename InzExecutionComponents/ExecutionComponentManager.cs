using InzExecutionComponents.Contracts;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Exception;
using InzExecutionComponents.ExecutionEvent;
using InzExecutionComponents.ExecutionPlan;

namespace InzExecutionComponents;

internal class ExecutionComponentManager : IExecutionComponentManager
{
    private readonly ExecutionPlanEngine _executionPlanEngine;
    private readonly ExecutionEventEngine _executionEventEngine;
    private readonly IServiceProvider _serviceProvider;

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

    public async Task LaunchExecution(string label, IExecutionPlanParametersContract parameters)
    {
        var plan = _executionPlanEngine.GetExecutionPlan(label);

        try
        {
            await _executionPlanEngine.PerformExecution(plan, parameters);
        }
        catch (System.Exception e)
        {
            throw new ExecutionPlanException(plan, e);
        }
    }

    public async Task LaunchExecution(string label)
    {
        var plan = _executionPlanEngine.GetExecutionPlan(label);

        try
        {
            await _executionPlanEngine.PerformExecution(plan, null);
        }
        catch (System.Exception e)
        {
            throw new ExecutionPlanException(plan, e);
        }
    }
}