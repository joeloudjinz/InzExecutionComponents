using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.Contracts.Configuration;

public interface IExecutionConfigurationOptions
{
    public void LoadIntoContextMetadata(IExecutionContext context);
}