namespace InzExecutionEvent.Contracts.ExecutionNotification;

public interface IExecutionNotification
{
    public string Name { get; set; }
    public string[] RequiredStoreKeys { get; set; }
    public string[] ProvidedStoreKeys { get; set; }
}