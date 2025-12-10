namespace InzExecutionComponents.Contracts.Configuration;

public interface IExecutionConfigurationOptionsResolver
{
    public void TryBind(string label, object config);
}