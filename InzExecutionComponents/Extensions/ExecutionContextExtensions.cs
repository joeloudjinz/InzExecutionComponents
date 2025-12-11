using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.Extensions;

public static class ExecutionContextExtensions
{
    public static T GetInputData<T>(this IExecutionContext context)
    {
        return context.Store.Get<T>(context.ExecutionPlanInputDataKey);
    }
}