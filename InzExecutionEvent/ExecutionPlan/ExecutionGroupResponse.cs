using System.Collections.ObjectModel;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.Models;

namespace InzExecutionEvent.ExecutionPlan;

// TODO: Remove/disable execution group feature
public class ExecutionGroupResponse : IExecutionResponse
{
    public bool Successful { get; set; }
    public Dictionary<string, IEnumerable<object>>? Results { get; set; } = new();
    public object? Result { get; set; } = null; // I think this property is useless in case of using execution group
    public ReadOnlyCollection<ExecutionFailure> Errors { get; set; } = new(Array.Empty<ExecutionFailure>());
}