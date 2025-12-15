using InzExecutionComponents.Contracts;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Exception;
using InzExecutionComponents.ExecutionPlan;

namespace InzExecutionComponents;

internal class ExecutionComponentManager(ExecutionPlanEngine executionPlanEngine) : IExecutionComponentManager
{
    public async Task LaunchExecution(string label, IExecutionPlanParametersContract parameters)
    {
        var plan = executionPlanEngine.GetExecutionPlan(label);

        try
        {
            await executionPlanEngine.PerformExecution(plan, parameters);
        }
        catch (System.Exception e)
        {
            throw new ExecutionPlanException(plan, e);
        }
    }

    public async Task LaunchExecution(string label)
    {
        var plan = executionPlanEngine.GetExecutionPlan(label);

        try
        {
            await executionPlanEngine.PerformExecution(plan, null);
        }
        catch (System.Exception e)
        {
            throw new ExecutionPlanException(plan, e);
        }
    }
}