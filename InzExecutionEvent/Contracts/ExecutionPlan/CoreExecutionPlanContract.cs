namespace InzExecutionEvent.Contracts.ExecutionPlan;

public class CoreExecutionPlanContract : IExecutionPlanContract
{
    public Guid PlanId { get; set; }
    public string DependencyRegistrationKey { get; set; } = string.Empty;
    public Type ImplementationType { get; set; }
    public string ImplementationTypeId { get; set; } = string.Empty;
    public bool IsRegistered { get; set; } = true;
    public string ExecutionLabel { get; set; } = string.Empty;
    public string ExecutionGroup { get; set; } = string.Empty;
    public string MessagingQueueRequestKey { get; set; } = string.Empty;
    public bool ShouldRunBeforeDispatchingPreExecutionEventsTask { get; set; }
    public bool ShouldRunExecutionTask { get; set; }
    public bool ShouldRunAfterDispatchingPostExecutionEventsTask { get; set; }
    public string[][] RequiredPreExecutionEvents { get; set; } = [];
    public string[][] RequiredPostExecutionEvents { get; set; } = [];
    public bool HasInputData { get; set; }
    public bool ValidateInputData { get; set; }
    public Type InputDataType { get; set; }
    public Type? InputDataValidatorType { get; set; }
    public Type? OutputDataType { get; set; }
    public string[] RequiredExecutionConfigurations { get; set; } = [];
    public string[] ExecutionNotificationToPublish { get; set; } = [];
}