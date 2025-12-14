using System.Reflection;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionNotification;
using InzExecutionComponents.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InzExecutionComponents.ExecutionNotification;

internal class ExecutionNotificationEngine
{
    public IServiceProvider ServiceProvider { get; set; } = null!;
    private readonly Dictionary<string, IExecutionNotificationContract> _notificationsMap = new();

    public void StartEngine(IServiceProvider services)
    {
        ServiceProvider = services;
    }

    public void RegisterExecutionNotificationAndHandlers(Assembly assembly, IServiceCollection services)
    {
        var notifications = Scouters.ExecutionNotificationTypes(assembly);
        var notificationHandlersMap = CreateNotificationHandlersMap(assembly);

        foreach (var notificationType in notifications)
        {
            var executionNotificationAttribute = notificationType.GetCustomAttribute<ExecutionNotificationAttribute>();
            if (executionNotificationAttribute == null)
            {
                Console.Error.WriteLine($"Skipping registration of execution notification with type [{notificationType}] because of missing ExecutionNotification attribute.");
                continue;
            }

            if (!notificationHandlersMap.TryGetValue(executionNotificationAttribute.Name, out var notificationHandlerContracts))
            {
                Console.Error.WriteLine($"Skipping event [{executionNotificationAttribute.Name}] because no handler was found");
                continue;
            }

            foreach (var notificationHandlerContract in notificationHandlerContracts)
            {
                services.TryAddKeyedSingleton(notificationHandlerContract.ImplementationType, notificationHandlerContract.RegistrationKey, notificationHandlerContract.ImplementationType);
            }

            var contract = new ExecutionNotificationContract
            {
                NotificationName = executionNotificationAttribute.Name,
                Handlers = notificationHandlerContracts,
            };

            _notificationsMap.Add(executionNotificationAttribute.Name, contract);
        }
    }

    private Dictionary<string, IExecutionNotificationHandlerContract[]> CreateNotificationHandlersMap(Assembly assembly)
    {
        var handlers = Scouters.ExecutionNotificationHandlerTypes(assembly);
        var map = new Dictionary<string, IExecutionNotificationHandlerContract[]>();
        foreach (var handlerType in handlers)
        {
            if (handlerType.GetCustomAttribute<ExecutionNotificationHandlerAttribute>() is not { } attribute)
            {
                Console.Error.WriteLine($"Skipping registration of execution notification handler with type [{handlerType}] because of missing ExecutionNotificationHandler attribute.");
                continue;
            }

            if (handlerType.GetInterfaces().All(i => i != typeof(IExecutionNotificationHandler)))
            {
                throw new System.Exception($"Execution notification handler of type [{handlerType}] does not implement {nameof(IExecutionNotificationHandler)} interface.");
            }

            var contract = new ExecutionNotificationHandlerContract
            {
                Name = attribute.HandlerName,
                ImplementationType = handlerType,
                ImplementationTypeId = handlerType.Name
            };
            contract.RegistrationKey = $"{contract.Name}.{contract.ImplementationTypeId}";

            if (map.TryGetValue(attribute.NotificationName, out var handlerNames))
            {
                map[attribute.NotificationName] = handlerNames.Append(contract).ToArray();
                continue;
            }

            map.Add(attribute.NotificationName, [contract]);
        }

        return map;
    }

    public async Task HandleNotifications(IExecutionPlanContext context, string[] notifications)
    {
        var handlers = new List<IExecutionNotificationHandler>();
        foreach (var name in notifications)
        {
            if (!_notificationsMap.TryGetValue(name, out var notificationContract)) throw new System.Exception($"Execution notification [{name}] is not registered.");
            if (notificationContract.Handlers.Length == 0) throw new System.Exception($"Execution notification [{name}] has no registered handler.");

            foreach (var handlerContract in notificationContract.Handlers)
            {
                var handler = GetNotificationHandler(handlerContract);
                if (handler is null) throw new System.Exception($"Execution notification handler of type [{handlerContract.ImplementationType.Name}] was not found.");
                handlers.Add(handler);
            }
        }

        await Task.WhenAll(handlers.Select(h => h.Handle(context)));
    }

    private IExecutionNotificationHandler? GetNotificationHandler(IExecutionNotificationHandlerContract contract)
    {
        try
        {
            var handler = ServiceProvider.GetRequiredKeyedService(contract.ImplementationType, contract.RegistrationKey);
            return handler as IExecutionNotificationHandler;
        }
        catch
        {
            return null;
        }
    }

    // public async Task HandleNotifications(IExecutionContext context, string[] notifications)
    // {
    //     var handlers = new List<IExecutionNotificationHandler>();
    //     foreach (var name in notifications)
    //     {
    //         if (!_notificationsMap.ContainsKey(name)) throw new System.Exception($"System notification [{name}] is not registered.");
    //         if (!_notificationToHandlersMap.TryGetValue(name, out var notificationHandlerNames)) throw new System.Exception($"System notification [{name}] is not mapped to any handler.");
    //         if (notificationHandlerNames.IsEmpty)
    //         {
    //             Console.WriteLine($"System notification [{name}] doesn't have any handler!");
    //             continue;
    //         }
    //
    //         foreach (var handler in _notificationToHandlersMap[name])
    //         {
    //             if (!_notificationHandlersMap.TryGetValue(handler, out var notificationHandlers)) throw new System.Exception($"System notification handler [{handler}] is not registered.");
    //             handlers.Add(notificationHandlers);
    //         }
    //     }
    //
    //     await Task.WhenAll(handlers.Select(h => h.Handle(context)));
    // }
}