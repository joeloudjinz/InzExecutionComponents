using System.Reflection;
using InzExecutionComponents.Attributes;

namespace InzExecutionComponents.ExecutionEvent;

internal static class ExecutionEventUtility
{
    public static void ProcessAttributes(IExecutionEventContract contract)
    {
        var attributes = contract.ImplementationType.GetCustomAttributes().ToList();

        foreach (var attribute in attributes)
        {
            if (attribute is BaseExecutionEventAttribute baseExecutionEventAttribute)
            {
                contract.Name = baseExecutionEventAttribute.Name;
                contract.Type = baseExecutionEventAttribute.EventType;
                contract.RequiredStoreKeys = baseExecutionEventAttribute.RequiredStoreKeys;
                contract.RequiredMetadataKeys = baseExecutionEventAttribute.RequiredMetadataKeys;
                contract.RequiredConfigurations = baseExecutionEventAttribute.RequiredConfigurations;
                continue;
            }

            if (attribute is PublishExecutionNotificationsAttribute systemNotifications)
            {
                contract.RequiredSystemNotifications = systemNotifications.Notifications;
                continue;
            }

            // Register more data for the event by processing other attribute here ...
        }
    }
    
    public static string GenerateRegistrationKey(IExecutionEventContract contract)
    {
        var randomSuffix = Guid.NewGuid().ToString().Split("-")[^5];
        return $"{contract.Name}@{contract.ImplementationType.Name}#{randomSuffix}";
    }
}