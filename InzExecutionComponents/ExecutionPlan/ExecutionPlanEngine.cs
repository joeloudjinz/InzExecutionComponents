using System.Reflection;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Engines;
using InzExecutionComponents.ExecutionContext;
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

    private readonly List<IExecutionPlanContract> _registeredExecutionPlanContracts = [];

    public IServiceProvider ServiceProvider { get; set; } = null!;

    public void StartEngine(IServiceProvider services)
    {
        ServiceProvider = services;
    }

    public void RegisterExecutionPlans(Assembly assembly, IServiceCollection services)
    {
        var executionTimeRecorderKey = $"RegisterExecutionPlans() for {assembly.GetName().Name}";
        ExecutionTimeRecorder.Start(executionTimeRecorderKey);

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

            planContract.RegistrationKey = ExecutionPlanUtility.GenerateExecutionPlanRegistrationKey(planContract);
            ExecutionPlanUtility.ProcessPlanInputDataDetails(planContract);
            ExecutionPlanUtility.ProcessPlanOutputDataDetails(planContract);
            ExecutionPlanUtility.ProcessInterfaces(
                planContract,
                executionPlanType.GetInterfaces().Where(i => _processableExecutionInterfaces.Contains(i)).ToList()
            );

            _registeredExecutionPlanContracts.Add(planContract);
            services.AddKeyedSingleton(serviceType: executionPlanType, serviceKey: planContract.RegistrationKey, implementationType: executionPlanType);
        }

        ExecutionTimeRecorder.EndThenPrint(executionTimeRecorderKey);
    }

    public async Task PerformExecution(IExecutionPlanContract plan, IExecutionParametersContract? parameters)
    {
        var executionTimeRecorderKey = $"PerformExecution() for {plan.Label}";
        ExecutionTimeRecorder.Start(executionTimeRecorderKey);

        var context = new ExecutionContext.ExecutionContext
        {
            ExecutionPlanLabel = plan.Label,
            ExecutionPlanRegistrationKey = plan.RegistrationKey,
            ExecutionPlanInputDataKey = plan.InputDataKey,
            ExecutionPlanOutputDataKey = plan.OutputDataKey,
            MetaData = new ExecutionContextDataStore(),
            Store = new ExecutionContextDataStore(),
            Failures = new ExecutionContextFailureRepository()
        };

        if (plan.HasInputData && parameters is not null)
        {
            context.Store.Set(context.ExecutionPlanInputDataKey, parameters);
            LoadInputValuesIntoExecutionContextStore(context, plan, parameters);
        }

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

        ExecutionTimeRecorder.EndThenPrint(executionTimeRecorderKey);
    }

    private void LoadInputValuesIntoExecutionContextStore(IInternalExecutionContext context, IExecutionPlanContract plan, IExecutionParametersContract parameters)
    {
        if (plan.InputDataPropertiesDetailsForContextStore.Count == 0) return;

        foreach (var (key, data) in plan.InputDataPropertiesDetailsForContextStore)
        {
            context.Store.Set(key, data.InputDataPropertyDetails.GetValue(parameters));
        }
    }

    private void LoadOutputValuesIntoExecutionContextStore(IInternalExecutionContext context, IExecutionPlanContract plan, IExecutionResultContract parameters)
    {
        if (plan.OutputDataPropertiesDetailsForContextStore.Count == 0) return;

        foreach (var (key, data) in plan.OutputDataPropertiesDetailsForContextStore)
        {
            context.Store.Set(key, data.InputDataPropertyDetails.GetValue(parameters));
        }
    }

    private async Task CheckAndPublishSystemNotificationsOfExecutionPlan(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.ExecutionNotificationToPublish.Length == 0) return;
        await executionNotificationEngine.HandleNotifications(context, plan.ExecutionNotificationToPublish);
    }

    private async Task CheckAndRunAfterDispatchingPostExecutionEventsTask(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunAfterDispatchingPostExecutionEventsTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IPostEventsExecutionContract>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.RegistrationKey} instance is null."); // TODO improve this error message

        await instance.AfterDispatchingPostExecutionEvents(context);
    }

    private async Task CheckAndDispatchPostExecutionEvents(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredPostExecutionEvents.Length == 0) return;
        await executionEventEngine.DispatchEvents(context, plan.RequiredPostExecutionEvents);
    }

    private async Task CheckAndRunExecutionTask(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunExecutionTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IExecutionContract<IExecutionResultContract>>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.RegistrationKey} instance is null."); // TODO improve this error message

        // TODO this could throw exception and they can be unhandled by users so catch exceptions and create a generic failure result
        var result = await instance.Execute(context);

        if (result.IsError)
        {
            // TODO rethink why adding this failure into this context storage
            context.Failures.Add(plan.RegistrationKey, result.Failure!);
            return;
        }

        if (plan.HasOutputData && result.Result is not null) LoadOutputValuesIntoExecutionContextStore(context, plan, result.Result);
    }

    private async Task CheckAndDispatchPreExecutionEvents(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredPreExecutionEvents.Length == 0) return;
        await executionEventEngine.DispatchEvents(context, plan.RequiredPreExecutionEvents);
    }

    private async Task CheckAndRunBeforeDispatchingPreExecutionEventsTask(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunBeforeDispatchingPreExecutionEventsTask) return;

        var instance = GetExecutionPlanFromDependencyContainer<IPreEventsExecutionContract>(plan);
        if (instance is null) throw new NullReferenceException($"The execution plan {plan.RegistrationKey} instance is null."); // TODO improve this error message

        await instance.BeforeDispatchingPreExecutionEvents(context);
    }

    private void CheckAndLoadRequiredConfigurationOptions(IInternalExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequiredExecutionConfigurations.Length == 0) return;
        executionConfigurationEngine.LoadConfigurationOptionsIntoContext(context, plan.RequiredExecutionConfigurations);
    }

    private static bool CheckAndProcessFailures(IInternalExecutionContext context)
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

    public IExecutionPlanContract GetExecutionPlan(string label)
    {
        var plan = _registeredExecutionPlanContracts.FirstOrDefault(p => p.Label.Equals(label));
        return plan ?? throw new InvalidOperationException($"Execution plan {label} was not found");
    }
}