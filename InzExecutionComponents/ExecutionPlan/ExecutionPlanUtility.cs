using System.Reflection;
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Enums;

namespace InzExecutionComponents.ExecutionPlan;

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

                contract.Label = executor.Name;
                contract.Group = executor.Group;
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
                contract.HasOutputData = true;
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
        // TODO Find a way to remove the use of generic result interface here so Execute() can be ran by the engine  
        planContract.ShouldRunExecutionTask = interfaces.Contains(typeof(IExecutionContract<IExecutionResultContract>));
        planContract.ShouldRunAfterDispatchingPostExecutionEventsTask = interfaces.Contains(typeof(IPostEventsExecutionContract));
    }

    public static void ProcessPlanInputDataDetails(IExecutionPlanContract planContract)
    {
        if (!planContract.HasInputData) return;

        if (planContract.InputDataType.GetInterfaces().All(i => i == typeof(IExecutionParametersContract)))
        {
            throw new InvalidOperationException($"Execution plan [{planContract.Label}] input data type does not implement {nameof(IExecutionParametersContract)}");
        }

        planContract.InputDataKey = GenerateExecutionPlanParametersKey(planContract);
        planContract.InputDataPropertiesDetailsForContextStore = planContract.InputDataType.GetProperties()
            .Where(p => p.GetCustomAttributes<ExecutionContextStoreKeyAttribute>().Any())
            .Select(p => new ExecutionPlanDataDetailsForContextStoreModel
            {
                ExecutionContextStoreKeyAttribute = p.GetCustomAttribute<ExecutionContextStoreKeyAttribute>()!,
                InputDataPropertyDetails = p
            })
            .ToDictionary(kv => kv.ExecutionContextStoreKeyAttribute.Key);
    }

    public static void ProcessPlanOutputDataDetails(IExecutionPlanContract planContract)
    {
        if (!planContract.HasOutputData) return;

        if (planContract.OutputDataType!.GetInterfaces().All(i => i == typeof(IExecutionResultContract)))
        {
            throw new InvalidOperationException($"Execution plan [{planContract.Label}] input data type does not implement {nameof(IExecutionResultContract)}");
        }

        planContract.OutputDataKey = GenerateExecutionPlanResultKey(planContract);
        planContract.OutputDataPropertiesDetailsForContextStore = planContract.OutputDataType!.GetProperties()
            .Where(p => p.GetCustomAttributes<ExecutionContextStoreKeyAttribute>().Any())
            .Select(p => new ExecutionPlanDataDetailsForContextStoreModel
            {
                ExecutionContextStoreKeyAttribute = p.GetCustomAttribute<ExecutionContextStoreKeyAttribute>()!,
                InputDataPropertyDetails = p
            })
            .ToDictionary(kv => kv.ExecutionContextStoreKeyAttribute.Key);
    }

    public static string GenerateExecutionPlanRegistrationKey(CoreExecutionPlanContract planContract)
    {
        var randomSuffix = Guid.NewGuid().ToString().Split("-")[^5];
        return $"{planContract.Label}@{planContract.ImplementationTypeId}#{randomSuffix}";
    }

    private static string GenerateExecutionPlanResultKey(IExecutionPlanContract plan) => $"{plan.RegistrationKey}.Result";

    private static string GenerateExecutionPlanParametersKey(IExecutionPlanContract plan) => $"{plan.RegistrationKey}.Params";
}