using Demo.DummyApp.Resources;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Contracts.Models;

namespace Demo.DummyApp.ExecutionPlans;

[ExecutionPlan(ExecutionPlanKeys.Test)]
[ExecutionInputDataType<TestOneInputData>]
[ExecutionOutputDataType<TestOneOutputData>]
[RegisterPreExecutionEvents(ExecutionEventKeys.TestEvent1)]
[RegisterPreExecutionEvents(ExecutionEventKeys.TestEvent2, ExecutionEventKeys.TestEvent3)]
[RegisterPostExecutionEvents(ExecutionEventKeys.TestEvent4)]
[PublishExecutionNotifications([ExecutionNotificationKeys.Test])]
public class TestExecutionPlan : IExecutionContract<IExecutionPlanResultContract>, IPreEventsExecutionContract, IPostEventsExecutionContract
{
    public async Task BeforeDispatchingPreExecutionEvents(IExecutionPlanContext context)
    {
        await Task.Delay(2000);
        Console.WriteLine($"{GetType().Name} - BeforeDispatchingPreExecutionEvents()");
    }

    public async Task<ExecutionResult<IExecutionPlanResultContract>> Execute(IExecutionPlanContext context)
    {
        var data = context.GetInputData<TestOneInputData>();

        Console.WriteLine($"{GetType().Name} - data:");
        Console.WriteLine($"{GetType().Name}    -> {data.One}");
        Console.WriteLine($"{GetType().Name}    -> {data.Two}");
        Console.WriteLine($"{GetType().Name}    -> {data.Three}");

        await Task.Delay(1500);

        return new TestOneOutputData
        {
            One = "one",
            Two = false,
            Three = 3
        };
    }

    public async Task AfterDispatchingPostExecutionEvents(IExecutionPlanContext context)
    {
        await Task.Delay(2000);
        Console.WriteLine($"{GetType().Name} - AfterDispatchingPostExecutionEvents()");
    }
}

public record TestOneInputData : IExecutionPlanParametersContract
{
    public string One { get; set; } = string.Empty;
    public bool Two { get; set; }
    public int Three { get; set; }
}

public record TestOneOutputData : IExecutionPlanResultContract
{
    public string One { get; set; } = string.Empty;
    public bool Two { get; set; }
    public int Three { get; set; }
}