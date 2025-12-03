namespace InzExecutionEvent.Contracts.ExecutionPlan;

public interface IExecutionGroupContract
{
    public string Key { get; set; }
    public string Type { get; set; }
    public string Route { get; set; }
    public string Method { get; set; }
    public List<string> Permissions { get; set; }
    public bool RequireAuthentication { get; set; }
    public bool RunOnMessagingQueue { get; set; }
    public bool UseRateLimiter { get; set; }
    public bool ExecuteInParallel { get; set; }
    public List<string> SystemNotifications { get; set; }
    public List<string> Plans { get; set; }
}