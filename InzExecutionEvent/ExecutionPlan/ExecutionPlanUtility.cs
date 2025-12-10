using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.ExecutionEvent;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Enums;

namespace InzExecutionEvent.ExecutionPlan;

public static class ExecutionPlanUtility
{
    public static void ProcessAttributes(IExecutionPlanContract contract, ICollection<object> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (attribute is ExecutionPlanAttribute executor)
            {
                // TODO check that environment is production in order to skip registering the execution plan
                contract.IsRegistered = executor.ExecutionPlanType != ExecutionPlanType.Test;
                if (!contract.IsRegistered) return;

                contract.ExecutionLabel = executor.Name;
                contract.ExecutionGroup = executor.Group;
                if (string.IsNullOrEmpty(contract.MessagingQueueRequestKey)) contract.MessagingQueueRequestKey = executor.Name;
                continue;
            }

            if (attribute is MessagingQueueLabelAttribute queueLabel)
            {
                contract.MessagingQueueRequestKey = queueLabel.Label;
                continue;
            }

            if (attribute is ExecutionInputDataTypeAttribute executionInputDataTypeAttribute)
            {
                contract.HasInputData = true;
                contract.InputDataType = executionInputDataTypeAttribute.RequestDataType;
                contract.ValidateInputData = executionInputDataTypeAttribute.ValidatorType is not null;
                if (contract.ValidateInputData) contract.InputDataValidatorType = executionInputDataTypeAttribute.RequestDataType;
                continue;
            }

            if (attribute is ExecutionConfigurationOptionsAttribute executionConfigurationOptionsAttribute)
            {
                contract.RequiredExecutionConfigurations = executionConfigurationOptionsAttribute.Labels;
                continue;
            }

            if (attribute is PublishExecutionNotificationsAttribute publishExecutionNotificationsAttribute)
            {
                contract.ExecutionNotificationToPublish = publishExecutionNotificationsAttribute.Notifications;
                continue;
            }

            if (attribute is ExecutionOutputDataTypeAttribute executionOutputDataTypeAttribute)
            {
                contract.OutputDataType = executionOutputDataTypeAttribute.Type;
                continue;
            }

            if (attribute is RegisterPreExecutionEvents registerPreExecutionEventsAttribute)
            {
                contract.RequiredPreExecutionEvents = contract.RequiredPreExecutionEvents.Append(registerPreExecutionEventsAttribute.Events).ToArray();
                continue;
            }

            if (attribute is RegisterPostExecutionEvents registerPostExecutionEventsAttribute)
            {
                contract.RequiredPostExecutionEvents = contract.RequiredPostExecutionEvents.Append(registerPostExecutionEventsAttribute.Events).ToArray();
            }
        }
    }

    public static void ProcessInterfaces(IExecutionPlanContract planContract, ICollection<Type> interfaces)
    {
        planContract.ShouldRunBeforeDispatchingPreExecutionEventsTask = interfaces.Contains(typeof(IPreEventsExecutionContract));
        planContract.ShouldRunExecutionTask = interfaces.Contains(typeof(IExecutionContract<IExecutionResultContract>));
        planContract.ShouldRunAfterDispatchingPostExecutionEventsTask = interfaces.Contains(typeof(IPostEventsExecutionContract));
    }

    // public static void HasRequestData(IExecutionContext context, IExecutionPlanContract plan)
    // {
    //     context.MetaData.Set(InzMetaDataKeys.Request.RawBodyString, plan.Body);
    //     context.MetaData.Set(InzMetaDataKeys.Request.BodyType, plan.InputDataType);
    //     plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.DeserializeRequestDataEvent]);
    //     if (plan.ValidateInputData)
    //     {
    //         // TODO: put context resources for request data validator event
    //         // plan.RequestEventsQueue.Enqueue(new[] {EventNames.ValidateRequestDataEvent});
    //     }
    // }
    //
    // public static void RequirePermissionsCheck(IExecutionContext context, IExecutionPlanContract plan)
    // {
    //     plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.LoadUserPermissionsCacheEvent]);
    //     context.MetaData.Set(InzMetaDataKeys.Request.PermissionsToCheck, plan.Permissions);
    //     plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.CheckUserHasPermissionsEvent]);
    // }
}