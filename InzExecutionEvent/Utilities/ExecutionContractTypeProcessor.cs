using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Utilities;

public static class ExecutionContractTypeProcessor
{
    public static void ProcessAttributes(IExecutionPlanContract contract, ICollection<object> attributes)
    {
        foreach (var attribute in attributes)
        {
            // TODO check that environment == EnvironmentModeEnum.Production in order to un-register the execution plan
            if (attribute is ExecutionPlanTypeAttribute { ExecutionPlanType: ExecutionPlanType.Test } _)
            {
                contract.IsRegistered = false;
                return;
            }

            if (attribute is ExecutorAttribute executor)
            {
                contract.ExecutionGroup = executor.Group;
                contract.ExecutionLabel = executor.Plan;
                continue;
            }

            if (attribute is ApiEndpointAttribute endpoint)
            {
                contract.UseRateLimiter = endpoint.RateLimited;
                contract.EndpointRoute = endpoint.Route;
                contract.EndpointMethod = endpoint.Method;
                continue;
            }

            if (attribute is MessagingQueueLabelAttribute queueLabel)
            {
                contract.MessagingQueueRequestKey = queueLabel.Label;
                continue;
            }

            if (attribute is RequireAuthenticationAttribute _)
            {
                contract.RequireAuthentication = true;
                continue;
            }

            if (attribute is RequirePermissionCheckAttribute permissions)
            {
                contract.RequirePermissionCheck = true;
                contract.Permissions = permissions.Permissions.ToList();
                continue;
            }

            if (attribute is RequestDataTypeAttribute requestDataType)
            {
                contract.HasRequestData = true;
                contract.RequestDataType = requestDataType.RequestDataType;
                contract.ValidateRequestData = requestDataType.ValidatorType is not null;
                if (contract.ValidateRequestData) contract.RequestDataValidatorType = requestDataType.RequestDataType;
                continue;
            }

            if (attribute is RequireConfigurationOptionsAttribute requireConfigurationOptions)
            {
                contract.RequiredConfigurations = requireConfigurationOptions.Labels;
                continue;
            }

            if (attribute is PublishSystemNotificationsAttribute publishSystemNotifications)
            {
                contract.SystemNotificationToPublish = publishSystemNotifications.Notifications;
                continue;
            }

            if (attribute is ResponseDataTypeAttribute responseDataType) contract.ResponseDataType = responseDataType.Type;
        }
    }

    public static void ProcessInterfaces(IExecutionPlanContract planContract, ICollection<Type> interfaces)
    {
        planContract.ShouldRunBeforeDispatchingPreExecutionEventsTask = interfaces.Contains(typeof(IPreEventsExecutionContract));
        planContract.ShouldRunExecutionTask = interfaces.Contains(typeof(IExecutionContract<IExecutionResultContract>));
        planContract.ShouldRunAfterDispatchingPostExecutionEventsTask = interfaces.Contains(typeof(IPostEventsExecutionContract));
        planContract.HasEvents = interfaces.Contains(typeof(IExecutionEventsContract));
    }
}