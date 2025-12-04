using System.Collections.Immutable;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionNotification;

namespace InzExecutionEvent.Engines;

internal class ExecutionNotificationEngine
{
    private Dictionary<string, IExecutionNotification> _notificationsMap = new();
    private Dictionary<string, IExecutionNotificationHandler> _notificationHandlersMap = new();
    private Dictionary<string, ImmutableList<string>> _notificationToHandlersMap = new();

    public void RegisterNotificationAndHandlers(ICollection<IExecutionNotification> registeredNotifications, ICollection<IExecutionNotificationHandler> registeredHandlers)
    {
        _notificationsMap = registeredNotifications.ToDictionary(rn => rn.Name);
        _notificationHandlersMap = registeredHandlers.ToDictionary(rh => rh.HandlerName);
        _notificationToHandlersMap = registeredHandlers.GroupBy(rh => rh.NotificationName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(rh => rh.HandlerName).ToImmutableList()
            );
    }

    public async Task HandleNotifications(IExecutionContext context, string[] notifications)
    {
        var handlers = new List<IExecutionNotificationHandler>();
        foreach (var name in notifications)
        {
            if (!_notificationsMap.ContainsKey(name)) throw new Exception($"System notification [{name}] is not registered.");
            if (!_notificationToHandlersMap.ContainsKey(name)) throw new Exception($"System notification [{name}] is not mapped to any handler.");
            if (!_notificationToHandlersMap[name].Any())
            {
                Console.WriteLine($"System notification [{name}] doesn't have any handler!");
                continue;
            }

            foreach (var handler in _notificationToHandlersMap[name])
            {
                if (!_notificationHandlersMap.ContainsKey(handler)) throw new Exception($"System notification handler [{handler}] is not registered.");
                handlers.Add(_notificationHandlersMap[handler]);
            }
        }

        await Task.WhenAll(handlers.Select(h => h.Handle(context)));
    }
}