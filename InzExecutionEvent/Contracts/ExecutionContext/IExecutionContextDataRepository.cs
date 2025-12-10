namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface IExecutionContextDataRepository
{
    public T? Get<T>(string key, T? defaultValue);
    public T Get<T>(string key);
    public void Set<T>(string key, T data);
    public (bool success, string missing) Check(ICollection<string> keys);
    public bool Has(string key);
    public bool Remove(string key);
    public void RemoveRange(ICollection<string> keys);
}