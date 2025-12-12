using System.Reflection;
using InzExecutionComponents.Attributes;

namespace InzExecutionComponents.ExecutionPlan;

public record ExecutionPlanDataDetailsForContextStoreModel
{
    public ExecutionContextStoreKeyAttribute ExecutionContextStoreKeyAttribute { get; set; } = null!;
    public PropertyInfo InputDataPropertyDetails { get; set; } = null!;
}