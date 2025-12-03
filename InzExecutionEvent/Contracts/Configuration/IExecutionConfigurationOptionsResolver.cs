namespace InzExecutionEvent.Contracts.Configuration;

public interface IExecutionConfigurationOptionsResolver
{
    public void TryBind(string label, object config);
}