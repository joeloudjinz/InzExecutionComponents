using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.ExecutionContext;

public class ExecutionContextMetaDataStore : IExecutionContextDataRepository
{
    private readonly Dictionary<string, dynamic> _storedData = new();

    public T? Get<T>(string key, T? defaultValue)
    {
        if (_storedData.ContainsKey(key)) return _storedData[key];
        if (!_storedData.ContainsKey(key) && defaultValue is not null) return defaultValue;
        return default;
    }

    public T Get<T>(string key)
    {
        if (!_storedData.ContainsKey(key)) throw new Exception($"Missing key [{key}] in context metadata repository.");
        return _storedData[key];
    }

    public void Set<T>(string key, T data)
    {
        if (data is null) return;
        _storedData[key] = data;
    }

    public (bool success, string missing) Check(ICollection<string> keys)
    {
        foreach (var key in keys)
            if (!Has(key))
                return (false, key);

        return (true, string.Empty);
    }

    public bool Has(string key)
    {
        return _storedData.ContainsKey(key);
    }

    public void Remove(string key)
    {
        if (!Has(key)) return;
        _storedData.Remove(key);
    }

    public void RemoveRange(ICollection<string> keys)
    {
        foreach (var key in keys) Remove(key);
    }
}