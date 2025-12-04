using Demo.DummyApp.Resources;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Contracts.Models;

namespace Demo.DummyApp.ExecutionPlans;

[ExecutionPlan("execution.plan.test")]
[MessagingQueueLabel("messaging-queue.execution.plan.test")]
[ExecutionInputDataType(typeof(TestOneInputData))]
[ExecutionOutputDataType(typeof(TestOneOutputData))]
[RegisterPreExecutionEvents(
    [ExecutionEventKeys.TestEvent1],
    [ExecutionEventKeys.TestEvent2, ExecutionEventKeys.TestEvent3]
)]
[RegisterPostExecutionEvents([ExecutionEventKeys.TestEvent4])]
[PublishExecutionNotifications([ExecutionNotificationKeys.Test])]
// [ExecutionConfigurationOptions([ConfigurationLabels.Test])]
public class TestExecutionPlan : IExecutionContract<IExecutionResultContract>, IPreEventsExecutionContract, IPostEventsExecutionContract
{
    public async Task BeforeDispatchingPreExecutionEvents(IExecutionContext context)
    {
        await Task.Delay(2000);
        Console.WriteLine($"{GetType().Name} - BeforeDispatchingPreExecutionEvents()");
    }

    public async Task<ExecutionResult<IExecutionResultContract>> Execute(IExecutionContext context)
    {
        // TODO get the input data

        await Task.Delay(2500);

        return new TestOneOutputData
        {
            One = "one",
            Two = false,
            Three = 3
        };
    }

    public async Task AfterDispatchingPostExecutionEvents(IExecutionContext context)
    {
        await Task.Delay(2000);
        Console.WriteLine($"{GetType().Name} - AfterDispatchingPostExecutionEvents()");
    }
}

public record TestOneInputData : IExecutionResultContract
{
    public string One { get; set; } = string.Empty;
    public bool Two { get; set; }
    public int Three { get; set; }
}

public record TestOneOutputData : IExecutionResultContract
{
    public string One { get; set; } = string.Empty;
    public bool Two { get; set; }
    public int Three { get; set; }
}