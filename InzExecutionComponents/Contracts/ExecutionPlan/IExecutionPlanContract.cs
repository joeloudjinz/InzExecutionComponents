using InzExecutionComponents.ExecutionPlan;

namespace InzExecutionComponents.Contracts.ExecutionPlan;

public interface IExecutionPlanContract
{
    public string RegistrationKey { get; set; }
    public Type ImplementationType { get; set; }
    public bool IsRegistered { get; set; }
    public string Label { get; set; }
    public string? Group { get; set; }
    public bool HasInputData { get; set; }
    public string InputDataKey { get; set; }
    public bool ValidateInputData { get; set; }
    public Type InputDataType { get; set; }
    public Type? InputDataValidatorType { get; set; }
    Dictionary<string, ExecutionPlanDataDetailsForContextStoreModel> InputDataPropertiesDetailsForContextStore { get; set; }
    public bool HasOutputData { get; set; }
    public string? OutputDataKey { get; set; }
    public Type? OutputDataType { get; set; }
    Dictionary<string, ExecutionPlanDataDetailsForContextStoreModel> OutputDataPropertiesDetailsForContextStore { get; set; }
    public string MessagingQueueRequestKey { get; set; }
    public bool ShouldRunBeforeDispatchingPreExecutionEventsTask { get; set; }
    public bool ShouldRunExecutionTask { get; set; }
    public bool ShouldRunAfterDispatchingPostExecutionEventsTask { get; set; }
    public string[][] RequiredPreExecutionEvents { get; set; }
    public string[][] RequiredPostExecutionEvents { get; set; }
    public string[] RequiredExecutionConfigurations { get; set; }
    public string[] ExecutionNotificationToPublish { get; set; }
}