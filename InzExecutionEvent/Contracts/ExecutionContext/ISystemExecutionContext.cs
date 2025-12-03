namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface ISystemExecutionContext : IExecutionContext
{
    public IExecutionResponse Response { get; set; }
}