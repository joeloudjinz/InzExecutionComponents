using System.Collections.Concurrent;
using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.ExecutionContext;

public class ExecutionContextDataStore : IExecutionContextDataRepository
{
    private readonly ConcurrentDictionary<string, object> _storedData = new();

    public T? Get<T>(string key, T? defaultValue)
    {
        if (!_storedData.TryGetValue(key, out var value) || value is not T typedValue) return defaultValue;
        return typedValue;
    }

    public T Get<T>(string key)
    {
        if (!_storedData.TryGetValue(key, out var value)) throw new KeyNotFoundException($"Missing key [{key}] in the context data store repository.");
        if (value is not T typedValue) throw new InvalidCastException($"Cannot cast value to type {typeof(T).Name}");
        return typedValue;
    }

    public void Set<T>(string key, T data)
    {
        if (data is null) throw new InvalidCastException("Context store repository should not contain null values, attempting to add null data.");
        _storedData[key] = data;
    }

    public (bool success, string missing) Check(ICollection<string> keys)
    {
        foreach (var key in keys)
        {
            if (!Has(key)) return (false, key);
        }

        return (true, string.Empty);
    }

    public bool Has(string key)
    {
        return _storedData.ContainsKey(key);
    }

    public bool Remove(string key) => _storedData.TryRemove(key, out _);

    public void RemoveRange(ICollection<string> keys)
    {
        foreach (var key in keys) Remove(key);
    }
}