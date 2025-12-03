using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Engines;
using InzExecutionEvent.ExecutionContext;
using InzExecutionEvent.Utilities;

namespace InzExecutionEvent.ExecutionPlan;

[ProvideSingleton(typeof(ExecutionPlanEngine))]
public class ExecutionPlanEngine(
    EventEngine eventEngine,
    ExecutionNotificationEngine executionNotificationEngine,
    ExecutionConfigurationEngine executionConfigurationEngine)
{
    private Dictionary<Guid, IExecutionRegistryContract> RegisteredExecutionPlanInstancesMap { get; } = new();

    private readonly List<Type> _processableAttributes =
    [
        typeof(ExecutionPlanTypeAttribute),
        typeof(ApiEndpointAttribute),
        typeof(MessagingQueueLabelAttribute),
        typeof(RequestDataTypeAttribute),
        typeof(ResponseDataTypeAttribute),
        typeof(RequirePermissionCheckAttribute),
        typeof(RequireConfigurationOptionsAttribute),
        typeof(PublishSystemNotificationsAttribute),
        typeof(ExecutorAttribute)
    ];

    private readonly List<Type> _processableExecutionInterfaces =
    [
        typeof(IExecutionEventsContract),
        typeof(IPreEventsExecutionContract),
        typeof(IExecutionContract<IExecutionResultContract>),
        typeof(IPostEventsExecutionContract)
    ];

    public List<IExecutionPlanContract> RegisteredExecutionPlanContracts { get; } = [];

    public void RegisterExecutionPlans(IEnumerable<IExecutionRegistryContract> executionContracts)
    {
        foreach (var executionContract in executionContracts)
        {
            var dataPlanContract = CreateExecutionDataContractFromExecutionContractInstance(executionContract);
            if (!dataPlanContract.IsRegistered) continue;
            RegisteredExecutionPlanInstancesMap.Add(dataPlanContract.PlanId, executionContract);
        }
    }

    public async Task LoadAndDispatchRequestExecutionEvents(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (plan.RequireAuthentication) ExecutionPlanUtility.RequiresAuthentication(context, plan);
        if (plan.RequirePermissionCheck) ExecutionPlanUtility.RequirePermissionsCheck(context, plan);
        if (plan.HasRequestData) ExecutionPlanUtility.HasRequestData(context, plan);
        await eventEngine.DispatchEvents(context, plan.RequestEventsQueue);
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
        if (!plan.SystemNotificationToPublish.Any()) return;
        await executionNotificationEngine.HandleNotifications(context, plan.SystemNotificationToPublish);
    }

    private async Task CheckAndRunAfterDispatchingPostExecutionEventsTask(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunAfterDispatchingPostExecutionEventsTask) return;
        if (RegisteredExecutionPlanInstancesMap[plan.PlanId] is not IPostEventsExecutionContract instance)
        {
            throw new NullReferenceException($"The execution plan {plan.PlanId} instance is null.");
        }

        await instance.AfterDispatchingPostExecutionEvents(context);
    }

    private async Task CheckAndDispatchPostExecutionEvents(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.HasEvents) return;
        var eventsContract = (IExecutionEventsContract)RegisteredExecutionPlanInstancesMap[plan.PlanId];
        if (eventsContract.PostExecutionEvents.Length == 0) return;
        await eventEngine.DispatchEvents(context, eventsContract.PostExecutionEvents);
    }

    private async Task CheckAndRunExecutionTask(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunExecutionTask) return;
        if (RegisteredExecutionPlanInstancesMap[plan.PlanId] is not IExecutionContract<IExecutionResultContract> instance)
        {
            throw new NullReferenceException($"The execution plan {plan.PlanId} instance is null.");
        }

        await instance.Execute(context);
    }

    private async Task CheckAndDispatchPreExecutionEvents(ISystemExecutionContext context, IExecutionPlanContract plan
    )
    {
        if (!plan.HasEvents) return;
        var eventsContract = (IExecutionEventsContract)RegisteredExecutionPlanInstancesMap[plan.PlanId];
        if (eventsContract.PreExecutionEvents.Length == 0) return;
        await eventEngine.DispatchEvents(context, eventsContract.PreExecutionEvents);
    }

    private async Task CheckAndRunBeforeDispatchingPreExecutionEventsTask(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.ShouldRunBeforeDispatchingPreExecutionEventsTask) return;
        if (RegisteredExecutionPlanInstancesMap[plan.PlanId] is not IPreEventsExecutionContract instance)
        {
            throw new NullReferenceException($"The execution plan {plan.PlanId} instance is null.");
        }

        await instance.BeforeDispatchingPreExecutionEvents(context);
    }

    private void CheckAndLoadRequiredConfigurationOptions(ISystemExecutionContext context, IExecutionPlanContract plan)
    {
        if (!plan.RequiredConfigurations.Any()) return;
        executionConfigurationEngine.LoadConfigurationOptionsIntoContext(context, plan.RequiredConfigurations);
    }

    private IExecutionPlanContract CreateExecutionDataContractFromExecutionContractInstance(IExecutionRegistryContract contract)
    {
        var planContract = new Contracts.ExecutionPlan.CoreExecutionPlanContract { PlanId = Guid.NewGuid() };
        var contractType = contract.GetType();
        ExecutionContractTypeProcessor.ProcessAttributes(
            planContract,
            contractType.GetCustomAttributes(false).Where(a => _processableAttributes.Contains(a.GetType())).ToList()
        );
        if (!planContract.IsRegistered) return planContract;
        ExecutionContractTypeProcessor.ProcessInterfaces(
            planContract,
            contractType.GetInterfaces().Where(i => _processableExecutionInterfaces.Contains(i)).ToList()
        );
        RegisteredExecutionPlanContracts.Add(planContract);
        return planContract;
    }
}