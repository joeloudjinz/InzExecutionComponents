using System.Reflection;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Engines;
using InzExecutionEvent.ExecutionContext;
using InzExecutionEvent.ExecutionEvent;
using InzExecutionEvent.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent.ExecutionPlan;

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
                PlanId = Guid.NewGuid(),
                ImplementationType = executionPlanType,
                ImplementationTypeId = executionPlanType.Name
            };

            ExecutionPlanUtility.ProcessAttributes(
                planContract,
                executionPlanType.GetCustomAttributes(false).Where(a => _processableAttributes.Contains(a.GetType())).ToList()
            );
            if (!planContract.IsRegistered) continue;

            planContract.DependencyRegistrationKey = $"ExecutionPlan.{planContract.ImplementationTypeId}.{planContract.PlanId}";
            Console.WriteLine("[RegisterExecutionPlans()] ----> " + planContract.DependencyRegistrationKey);
            ExecutionPlanUtility.ProcessInterfaces(
                planContract,
                executionPlanType.GetInterfaces().Where(i => _processableExecutionInterfaces.Contains(i)).ToList()
            );
            RegisteredExecutionPlanContracts.Add(planContract);
            services.AddKeyedSingleton(serviceType: executionPlanType, serviceKey: planContract.DependencyRegistrationKey, implementationType: executionPlanType);
        }
    }

    public async Task PerformExecution(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        CheckAndLoadRequiredConfigurationOptions(context, plan);
        await CheckAndRunBeforeDispatchingPreExecutionEventsTask(context, plan);
        if (ExecutionContextStaticEngine.CheckAndProcessFailures(context)) return;
        await CheckAndDispatchPreExecutionEvents(context, plan);
        if (ExecutionContextStaticEngine.CheckAndProcessFailures(context)) return;
        await CheckAndRunExecutionTask(context, plan);
        if (ExecutionContextStaticEngine.CheckAndProcessFailures(context)) return;
        await CheckAndDispatchPostExecutionEvents(context, plan);
        if (ExecutionContextStaticEngine.CheckAndProcessFailures(context)) return;
        await CheckAndRunAfterDispatchingPostExecutionEventsTask(context, plan);
        if (ExecutionContextStaticEngine.CheckAndProcessFailures(context)) return;
        await CheckAndPublishSystemNotificationsOfExecutionPlan(context, plan);
    }

    private async Task CheckAndPublishSystemNotificationsOfExecutionPlan(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        return;
        // TODO enable execution notification feature
        if (plan.ExecutionNotificationToPublish.Length == 0) return;
        await executionNotificationEngine.HandleNotifications(context, plan.ExecutionNotificationToPublish);
    }

    private async Task CheckAndRunAfterDispatchingPostExecutionEventsTask(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunAfterDispatchingPostExecutionEventsTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IPostEventsExecutionContract>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.PlanId} instance is null."); // TODO improve this error message

        await instance.AfterDispatchingPostExecutionEvents(context);
    }

    private async Task CheckAndDispatchPostExecutionEvents(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredPostExecutionEvents.Length == 0) return;
        await executionEventEngine.DispatchEvents(context, plan.RequiredPostExecutionEvents);
    }

    private async Task CheckAndRunExecutionTask(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunExecutionTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IExecutionContract<IExecutionResultContract>>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.PlanId} instance is null."); // TODO improve this error message

        await instance.Execute(context);
    }

    private async Task CheckAndDispatchPreExecutionEvents(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredPreExecutionEvents.Length == 0) return;
        await executionEventEngine.DispatchEvents(context, plan.RequiredPreExecutionEvents);
    }

    private async Task CheckAndRunBeforeDispatchingPreExecutionEventsTask(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunBeforeDispatchingPreExecutionEventsTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IPreEventsExecutionContract>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.PlanId} instance is null."); // TODO improve this error message

        await instance.BeforeDispatchingPreExecutionEvents(context);
    }

    private void CheckAndLoadRequiredConfigurationOptions(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredExecutionConfigurations.Length == 0) return;
        executionConfigurationEngine.LoadConfigurationOptionsIntoContext(context, plan.RequiredExecutionConfigurations);
    }

    private T? GetExecutionPlanFromDependencyContainer<T>(IExecutionPlanContract plan)
    {
        try
        {
            return (T)ServiceProvider.GetRequiredKeyedService(plan.ImplementationType, plan.DependencyRegistrationKey);
        }
        catch
        {
            return default;
        }
    }
}