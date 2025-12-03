namespace InzExecutionEvent.Contracts.ExecutionPlan;

public interface IExecutionPlanContract
{
    public Guid PlanId { get; set; }
    public bool IsRegistered { get; set; }
    public string ExecutionLabel { get; set; }
    public string ExecutionGroup { get; set; }
    public Queue<string[]> RequestEventsQueue { get; set; }
    public bool RequirePermissionCheck { get; set; }
    public List<string> Permissions { get; set; }
    public bool HasRequestData { get; set; }
    public bool ValidateRequestData { get; set; }
    public Type RequestDataType { get; set; }
    public Type? RequestDataValidatorType { get; set; }
    public Type? ResponseDataType { get; set; }
    public bool UseRateLimiter { get; set; }
    public string EndpointRoute { get; set; }
    public HttpMethod EndpointMethod { get; set; }
    public bool RequireAuthentication { get; set; }
    public string AuthenticationHeader { get; set; }
    public string Body { get; set; }
    public string MessagingQueueRequestKey { get; set; }
    public bool ShouldRunBeforeDispatchingPreExecutionEventsTask { get; set; }
    public bool ShouldRunExecutionTask { get; set; }
    public bool ShouldRunAfterDispatchingPostExecutionEventsTask { get; set; }
    public bool HasEvents { get; set; }
    public string[] RequiredConfigurations { get; set; }
    public string[] SystemNotificationToPublish { get; set; }
}