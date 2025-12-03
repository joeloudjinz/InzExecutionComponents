using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Resources;

namespace InzExecutionEvent.Utilities;

public static class ExecutionGroupExtensions
{
    public static string GetHttpMethod(this IExecutionGroupContract contract)
    {
        return contract.Method switch
        {
            ExecutionGroupResources.Methods.Get => HttpMethod.Get.Method,
            ExecutionGroupResources.Methods.Post => HttpMethod.Post.Method,
            ExecutionGroupResources.Methods.Patch => HttpMethod.Patch.Method,
            ExecutionGroupResources.Methods.Delete => HttpMethod.Delete.Method,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static bool IsRegistered(this IExecutionGroupContract contract)
    {
        return contract.Type == ExecutionGroupResources.Types.Feature;
    }
}