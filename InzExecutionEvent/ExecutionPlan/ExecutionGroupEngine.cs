using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Engines;
using InzExecutionEvent.Resources;

namespace InzExecutionEvent.ExecutionPlan;

[ProvideSingleton(typeof(ExecutionGroupEngine))]
public class ExecutionGroupEngine(ExecutionPlanEngine executionPlanEngine, ExecutionNotificationEngine executionNotificationEngine)
{
    private readonly Dictionary<string, IExecutionGroupContract> _registeredExecutionGroupsMap = new();
    private readonly Dictionary<string, List<Guid>> _groupToPlansMap = new();
    public ICollection<IExecutionGroupContract> RegisteredExecutionGroups => _registeredExecutionGroupsMap.Values;

    public void Load(ICollection<IExecutionGroupContract> groups)
    {
        foreach (var group in groups)
        {
            if (_registeredExecutionGroupsMap.ContainsKey(group.Key)) throw new Exception($"Attempt to register execution group [{group.Key}] more than one time.");
            // TODO check env also if it production
            if (group.Type == ExecutionGroupResources.Types.Test) continue;
            if (!group.Plans.Any()) throw new Exception($"Execution group [{group.Key}] does not have execution plans.");
            _registeredExecutionGroupsMap.Add(group.Key, new CoreExecutionGroupContract
            {
                Key = group.Key,
                Type = group.Type,
                Route = group.Route,
                Method = group.Method,
                RequireAuthentication = group.RequireAuthentication,
                RunOnMessagingQueue = group.RunOnMessagingQueue,
                UseRateLimiter = group.UseRateLimiter,
                ExecuteInParallel = group.ExecuteInParallel,
                Plans = group.Plans,
                Permissions = group.Permissions,
                SystemNotifications = group.SystemNotifications,
            });
            _groupToPlansMap.Add(
                group.Key,
                executionPlanEngine.RegisteredExecutionPlanContracts.Where(c => group.Plans.Contains(c.ExecutionLabel))
                    .Select(c => c.PlanId)
                    .ToList()
            );
        }

        // _groupToPlansMap = _executionPlanEngine.RegisteredExecutionPlanContracts.GroupBy(e => e.ExecutionGroup)
        //     .ToDictionary(
        //         g => g.Key,
        //         g => g.Select(p => p.PlanId).ToList()
        //     );
    }

    public async Task PerformExecution(ISystemExecutionContext context, IExecutionGroupContract group)
    {
        if (!_registeredExecutionGroupsMap.ContainsKey(group.Key)) throw new Exception($"Execution group [{group.Key}] does not exist.");
        if (!_groupToPlansMap.TryGetValue(group.Key, out var plans) || !plans.Any()) throw new Exception($"Execution group [{group.Key}] does not have execution plans.");
        if (group.ExecuteInParallel) await PerformParallelExecution(context, plans);
        else await PerformSequentialExecution(context, plans);
        if (!context.Failures.HasFatal()) await CheckAndPublishSystemNotificationsOfExecutionPlan(context, group);
    }

    private async Task PerformSequentialExecution(ISystemExecutionContext context, ICollection<Guid> plans)
    {
        var tasks = executionPlanEngine.RegisteredExecutionPlanContracts
            .Where(p => plans.Contains(p.PlanId))
            .ToList();
        foreach (var task in tasks)
        {
            await executionPlanEngine.PerformExecution(context, task);
            if (context.Failures.HasFatal()) break;
        }
    }

    private async Task PerformParallelExecution(ISystemExecutionContext context, ICollection<Guid> plans)
    {
        await Task.WhenAll(
            executionPlanEngine.RegisteredExecutionPlanContracts
                .Where(p => plans.Contains(p.PlanId))
                .Select(p => executionPlanEngine.PerformExecution(context, p))
                .ToList()
        );
    }

    private async Task CheckAndPublishSystemNotificationsOfExecutionPlan(IExecutionContext context, IExecutionGroupContract group)
    {
        if (!group.SystemNotifications.Any()) return;
        await executionNotificationEngine.HandleNotifications(context, group.SystemNotifications.ToArray());
    }
}