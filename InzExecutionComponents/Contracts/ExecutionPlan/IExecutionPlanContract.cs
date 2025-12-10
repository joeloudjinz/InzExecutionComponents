namespace InzExecutionComponents.Contracts.ExecutionPlan;

public interface IExecutionPlanContract
{
    public string RegistrationKey { get; set; }
    public Type ImplementationType { get; set; }
    public bool IsRegistered { get; set; }
    public string ExecutionLabel { get; set; }
    public string? ExecutionGroup { get; set; }
    public bool HasInputData { get; set; }
    public bool ValidateInputData { get; set; }
    public Type InputDataType { get; set; }
    public Type? InputDataValidatorType { get; set; }
    public Type? OutputDataType { get; set; }
    public string MessagingQueueRequestKey { get; set; }
    public bool ShouldRunBeforeDispatchingPreExecutionEventsTask { get; set; }
    public bool ShouldRunExecutionTask { get; set; }
    public bool ShouldRunAfterDispatchingPostExecutionEventsTask { get; set; }
    public string[][] RequiredPreExecutionEvents { get; set; }
    public string[][] RequiredPostExecutionEvents { get; set; }
    public string[] RequiredExecutionConfigurations { get; set; }
    public string[] ExecutionNotificationToPublish { get; set; }
}