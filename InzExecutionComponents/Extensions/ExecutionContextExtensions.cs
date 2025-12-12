using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionPlan;

namespace InzExecutionComponents.Extensions;

public static class ExecutionContextExtensions
{
    public static T GetInputData<T>(this IExecutionContext context) where T : class, IExecutionParametersContract
    {
        if (string.IsNullOrEmpty(context.ExecutionPlanInputDataKey)) throw new ArgumentException("Execution plan input data key cannot be null or empty");
        return context.Store.Get<T>(context.ExecutionPlanInputDataKey);
    }

    public static void SetOutputData<T>(this IExecutionContext context, T data) where T : class, IExecutionResultContract
    {
        if (string.IsNullOrEmpty(context.ExecutionPlanOutputDataKey)) throw new ArgumentException("Execution plan output data key cannot be null or empty");
        context.Store.Set(context.ExecutionPlanOutputDataKey, data);
    }

    public static T GetOutputData<T>(this IExecutionContext context) where T : class, IExecutionResultContract
    {
        if (string.IsNullOrEmpty(context.ExecutionPlanOutputDataKey)) throw new ArgumentException("Execution plan output data key cannot be null or empty");
        return context.Store.Get<T>(context.ExecutionPlanOutputDataKey);
    }
}