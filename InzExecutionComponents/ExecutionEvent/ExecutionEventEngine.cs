using System.Reflection;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Exception;
using InzExecutionComponents.ExecutionNotification;
using InzExecutionComponents.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionComponents.ExecutionEvent;

internal class ExecutionEventEngine(ExecutionNotificationEngine executionNotificationEngine)
{
    private IServiceProvider ServiceProvider { get; set; } = null!;

    private readonly Dictionary<string, EventContract> _registeredEventsContracts = new();

    public void StartEngine(IServiceProvider services)
    {
        ServiceProvider = services;
    }

    public void RegisterExecutionEvents(Assembly assembly, IServiceCollection services)
    {
        var executionTimeRecorderKey = $"RegisterExecutionEvents() for {assembly.GetName().Name}";
        ExecutionTimeRecorder.Start(executionTimeRecorderKey);

        var types = Scouters.ExecutionEventTypes(assembly);
        foreach (var type in types)
        {
            var contract = new EventContract { ImplementationType = type };

            ExecutionEventUtility.ProcessAttributes(contract);
            contract.RegistrationKey = ExecutionEventUtility.GenerateRegistrationKey(contract);

            _registeredEventsContracts.Add(contract.Name, contract);
            services.AddKeyedSingleton(serviceType: contract.ImplementationType, serviceKey: contract.RegistrationKey, implementationType: contract.ImplementationType);
        }

        ExecutionTimeRecorder.EndThenPrint(executionTimeRecorderKey);
    }

    public async Task DispatchEvents(IInternalExecutionContext context, Queue<string[]> map)
    {
        foreach (var events in map)
        {
            await Task.WhenAll(events.Select(e => ProcessEventTypeAndDispatchEvent(context, e)));
            if (!context.Failures.HasFatal()) continue;
            break;
        }
    }

    // TODO improve the logic of this method
    public async Task DispatchEvents(IInternalExecutionContext context, string[][] map)
    {
        foreach (var events in map)
        {
            await Task.WhenAll(events.Select(e => ProcessEventTypeAndDispatchEvent(context, e)));
            if (!context.Failures.HasFatal()) continue;
            break;
        }
    }

    private async Task ProcessEventTypeAndDispatchEvent(IInternalExecutionContext context, string name)
    {
        if (!_registeredEventsContracts.TryGetValue(name, out var contract)) throw new InvalidOperationException($"Execution event [{name}] was not found");

        CheckIfEventRequiresContextMetadataResources(context, contract);
        CheckIfEventRequiresContextStoreResources(context, contract);
        // CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(context, name);

        var instance = GetExecutionEventFromDependencyContainer<IExecutionEvent>(contract);
        if (instance is null)
        {
            // TODO improve this error message
            throw new NullReferenceException($"Execution event with registration key [{contract.RegistrationKey}] was not found in the dependency injection container.");
        }

        try
        {
            await instance.Perform(context);
        }
        catch (System.Exception e)
        {
            throw new ExecutionEventException(name, contract, e);
        }

        await CheckIfEventPublishesSystemNotificationAndPublish(context, contract);
    }

    private void CheckIfEventRequiresContextMetadataResources(IInternalExecutionContext context, IExecutionEventContract contract)
    {
        if (contract.RequiredMetadataKeys.Length == 0) return;

        var results = context.MetaData.Check(contract.RequiredMetadataKeys);
        if (results.success) return;

        throw new MissingContextKeyException(storageType: "metadata", key: results.missing, contract);
    }

    private void CheckIfEventRequiresContextStoreResources(IInternalExecutionContext context, IExecutionEventContract contract)
    {
        if (contract.RequiredStoreKeys.Length == 0) return;

        var results = context.Store.Check(contract.RequiredStoreKeys);
        if (results.success) return;

        throw new MissingContextKeyException(storageType: "store", key: results.missing, contract);
    }

    private Task CheckIfEventPublishesSystemNotificationAndPublish(IInternalExecutionContext context, EventContract contract)
    {
        if (contract.RequiredSystemNotifications.Length == 0) return Task.CompletedTask;
        return executionNotificationEngine.HandleNotifications(context, contract.RequiredSystemNotifications);
    }

    // private void CheckIfEventRequiresConfigurationsAndLoadConfigurationsIntoContext(IInternalExecutionContext context, string name)
    // {
    //     if (model.RequiredConfigurations.Length == 0) return;
    //     configurationEngine.LoadConfigurationOptionsIntoContext(context, model.RequiredConfigurations);
    // }

    private T? GetExecutionEventFromDependencyContainer<T>(IExecutionEventContract e)
    {
        try
        {
            return (T)ServiceProvider.GetRequiredKeyedService(e.ImplementationType, e.RegistrationKey);
        }
        catch
        {
            return default;
        }
    }
}