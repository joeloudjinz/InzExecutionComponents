using System.Reflection;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Engines;
using InzExecutionComponents.ExecutionEvent;
using InzExecutionComponents.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionComponents.ExecutionPlan;

internal class ExecutionPlanEngine(
    ExecutionEventEngine executionEventEngine,
    ExecutionNotificationEngine executionNotificationEngine,
    ExecutionConfigurationEngine executionConfigurationEngine
)
{
    public IServiceProvider ServiceProvider { get; set; } = null!;

    private readonly List<Type> _processableAttributes =
    [
        typeof(MessagingQueueLabelAttribute),
        typeof(ExecutionInputDataTypeAttribute),
        typeof(ExecutionOutputDataTypeAttribute),
        typeof(ExecutionConfigurationOptionsAttribute),
        typeof(PublishExecutionNotificationsAttribute),
        typeof(ExecutionPlanAttribute),
        typeof(RegisterPreExecutionEvents),
        typeof(RegisterPostExecutionEvents)
    ];

    private readonly List<Type> _processableExecutionInterfaces =
    [
        typeof(IPreEventsExecutionContract),
        typeof(IExecutionContract<IExecutionResultContract>),
        typeof(IPostEventsExecutionContract)
    ];

    public List<IExecutionPlanContract> RegisteredExecutionPlanContracts { get; } = [];

    public void StartEngine(IServiceProvider services)
    {
        ServiceProvider = services;
    }

    public void RegisterExecutionPlans(Assembly assembly, IServiceCollection services)
    {
        var executionPlanTypes = Scouters.ExecutionPlanTypes(assembly);
        foreach (var executionPlanType in executionPlanTypes)
        {
            var planContract = new CoreExecutionPlanContract
            {
                ImplementationType = executionPlanType,
                ImplementationTypeId = executionPlanType.Name
            };

            ExecutionPlanUtility.ProcessAttributes(
                planContract,
                executionPlanType.GetCustomAttributes(false).Where(a => _processableAttributes.Contains(a.GetType())).ToList()
            );
            if (!planContract.IsRegistered) continue;
            
            var randomSuffix = Guid.NewGuid().ToString().Split("-")[^5];
            planContract.RegistrationKey = $"{planContract.ExecutionLabel}@{planContract.ImplementationTypeId}#{randomSuffix}";
            Console.WriteLine("[RegisterExecutionPlans()] ----> " + planContract.RegistrationKey);
            
            ExecutionPlanUtility.ProcessInterfaces(
                planContract,
                executionPlanType.GetInterfaces().Where(i => _processableExecutionInterfaces.Contains(i)).ToList()
            );
            RegisteredExecutionPlanContracts.Add(planContract);
            services.AddKeyedSingleton(serviceType: executionPlanType, serviceKey: planContract.RegistrationKey, implementationType: executionPlanType);
        }
    }

    public async Task PerformExecution(IExecutionContext context, IExecutionPlanContract plan)
    {
        CheckAndLoadRequiredConfigurationOptions(context, plan);
        await CheckAndRunBeforeDispatchingPreExecutionEventsTask(context, plan);
        if (CheckAndProcessFailures(context)) return;

        await CheckAndDispatchPreExecutionEvents(context, plan);
        if (CheckAndProcessFailures(context)) return;

        await CheckAndRunExecutionTask(context, plan);
        if (CheckAndProcessFailures(context)) return;

        await CheckAndDispatchPostExecutionEvents(context, plan);
        if (CheckAndProcessFailures(context)) return;

        await CheckAndRunAfterDispatchingPostExecutionEventsTask(context, plan);
        if (CheckAndProcessFailures(context)) return;

        await CheckAndPublishSystemNotificationsOfExecutionPlan(context, plan);
    }

    private async Task CheckAndPublishSystemNotificationsOfExecutionPlan(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.ExecutionNotificationToPublish.Length == 0) return;
        await executionNotificationEngine.HandleNotifications(context, plan.ExecutionNotificationToPublish);
    }

    private async Task CheckAndRunAfterDispatchingPostExecutionEventsTask(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunAfterDispatchingPostExecutionEventsTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IPostEventsExecutionContract>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.RegistrationKey} instance is null."); // TODO improve this error message

        await instance.AfterDispatchingPostExecutionEvents(context);
    }

    private async Task CheckAndDispatchPostExecutionEvents(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredPostExecutionEvents.Length == 0) return;
        await executionEventEngine.DispatchEvents(context, plan.RequiredPostExecutionEvents);
    }

    private async Task CheckAndRunExecutionTask(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunExecutionTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IExecutionContract<IExecutionResultContract>>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.RegistrationKey} instance is null."); // TODO improve this error message

        // TODO this could throw exception and they can be unhandled by users so catch exceptions and create a generic failure result
        var result = await instance.Execute(context);

        if (result.IsSuccess)
        {
            (context.Results as ISystemExecutionContextResultRepository)!.Add(plan.RegistrationKey, result);
            return;
        }

        context.Failures.Add(plan.RegistrationKey, result.Failure!);
    }

    private async Task CheckAndDispatchPreExecutionEvents(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredPreExecutionEvents.Length == 0) return;
        await executionEventEngine.DispatchEvents(context, plan.RequiredPreExecutionEvents);
    }

    private async Task CheckAndRunBeforeDispatchingPreExecutionEventsTask(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunBeforeDispatchingPreExecutionEventsTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IPreEventsExecutionContract>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.RegistrationKey} instance is null."); // TODO improve this error message

        await instance.BeforeDispatchingPreExecutionEvents(context);
    }

    private void CheckAndLoadRequiredConfigurationOptions(IExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredExecutionConfigurations.Length == 0) return;
        executionConfigurationEngine.LoadConfigurationOptionsIntoContext(context, plan.RequiredExecutionConfigurations);
    }

    private static bool CheckAndProcessFailures(IExecutionContext context)
    {
        return context.Failures.HasFailures() && context.Failures.HasFatal();
    }

    private T? GetExecutionPlanFromDependencyContainer<T>(IExecutionPlanContract plan)
    {
        try
        {
            return (T)ServiceProvider.GetRequiredKeyedService(plan.ImplementationType, plan.RegistrationKey);
        }
        catch
        {
            return default;
        }
    }
}