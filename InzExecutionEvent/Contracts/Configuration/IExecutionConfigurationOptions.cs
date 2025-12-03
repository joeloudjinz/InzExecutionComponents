using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.Contracts.Configuration;

public interface IExecutionConfigurationOptions
{
    public void LoadIntoContextMetadata(IExecutionContext context);
}