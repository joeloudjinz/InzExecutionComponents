namespace InzExecutionEvent.Contracts.ExecutionPlan;

public class CoreExecutionPlanContract : IExecutionPlanContract
{
    public Guid PlanId { get; set; }
    public bool IsRegistered { get; set; } = true;
    public string ExecutionLabel { get; set; } = string.Empty;
    public string ExecutionGroup { get; set; } = string.Empty;
    public string MessagingQueueRequestKey { get; set; } = string.Empty;
    public bool ShouldRunBeforeDispatchingPreExecutionEventsTask { get; set; }
    public bool ShouldRunExecutionTask { get; set; }
    public bool ShouldRunAfterDispatchingPostExecutionEventsTask { get; set; }
    public bool HasEvents { get; set; }
    public Queue<string[]> RequestEventsQueue { get; set; } = new();
    public bool RequirePermissionCheck { get; set; }
    public List<string> Permissions { get; set; } = [];
    public bool HasInputData { get; set; }
    public bool ValidateInputData { get; set; }
    public Type InputDataType { get; set; }
    public Type? InputDataValidatorType { get; set; }
    public Type? OutputDataType { get; set; }
    public bool UseRateLimiter { get; set; }
    public string[] RequiredExecutionConfigurations { get; set; } = [];
    public string[] ExecutionNotificationToPublish { get; set; } = [];
    public bool RequireAuthentication { get; set; }
    public string AuthenticationHeader { get; set; } = string.Empty;
    public string Body { get; set; }
    public string EndpointRoute { get; set; }
    public HttpMethod EndpointMethod { get; set; }
    public Dictionary<string, object?> RouteParams { get; set; } = new();
    public Dictionary<string, string> QueryParams { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
}