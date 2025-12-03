namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface IExecutionContext : IServiceExecutionContext
{
    public IExecutionContextDataRepository MetaData { get; set; }
    public IExecutionContextResultRepository Results { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
}