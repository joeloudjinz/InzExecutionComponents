using InzExecutionComponents.Contracts.ExecutionContext;

namespace InzExecutionComponents.Contracts.Configuration;

public interface IExecutionConfigurationOptions
{
    public void Load(IExecutionContextDataRepository metadata);
}