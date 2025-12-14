using InzExecutionComponents.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionPlan;

internal class ExecutionPlanContract : IExecutionPlanContract
{
    public string RegistrationKey { get; set; } = string.Empty;
    public Type ImplementationType { get; set; } = null!;
    public string ImplementationTypeId { get; set; } = string.Empty;
    public bool IsRegistered { get; set; } = true;
    public string Label { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string MessagingQueueRequestKey { get; set; } = string.Empty;
    public bool ShouldRunBeforeDispatchingPreExecutionEventsTask { get; set; }
    public bool ShouldRunExecutionTask { get; set; }
    public bool ShouldRunAfterDispatchingPostExecutionEventsTask { get; set; }
    public string[][] RequiredPreExecutionEvents { get; set; } = [];
    public string[][] RequiredPostExecutionEvents { get; set; } = [];
    public bool HasInputData { get; set; }
    public string InputDataKey { get; set; } = string.Empty;
    public bool ValidateInputData { get; set; }
    public Type? InputDataType { get; set; }
    public Type? InputDataValidatorType { get; set; }
    public Dictionary<string, ExecutionPlanDataDetailsForContextStoreModel> InputDataPropertiesDetailsForContextStore { get; set; } = new();
    public bool HasOutputData { get; set; }
    public string? OutputDataKey { get; set; }
    public Type? OutputDataType { get; set; }
    public Dictionary<string, ExecutionPlanDataDetailsForContextStoreModel> OutputDataPropertiesDetailsForContextStore { get; set; } = new();
    public string[] RequiredExecutionConfigurations { get; set; } = [];
    public string[] ExecutionNotificationToPublish { get; set; } = [];
}