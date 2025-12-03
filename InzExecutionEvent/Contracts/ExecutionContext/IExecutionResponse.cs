using System.Collections.ObjectModel;
using InzExecutionEvent.Contracts.Models;

namespace InzExecutionEvent.Contracts.ExecutionContext;

public interface IExecutionResponse
{
    public bool Successful { get; set; }
    public Dictionary<string, IEnumerable<object>>? Results { get; set; }
    public object? Result { get; set; }
    ReadOnlyCollection<ExecutionFailure> Errors { get; set; }
}