// using Inz.Common.Attributes;
// using Inz.Common.Contracts.Execution;
// using Inz.Common.Contracts.ExecutionContext;
// using Inz.Common.Resources;
// using Inz.Common.Enums;
// using Inz.Core.Engine.Contracts;
//
// namespace Inz.Core.Engine.ExecutionEngine;
//
// [ProvideSingleton(typeof(ExecutionEngineHelper))]
// public class ExecutionEngineHelper
// {
//     public void ExecutionPlanRequiresAuthentication(IExecutionContext context, IExecutionPlanContract plan)
//     {
//         context.MetaData.Set(InzMetaDataKeys.Request.RawBodyString, plan.AuthenticationHeader);
//         plan.RequestEventsQueue.Enqueue([ContextEventNames.DecodeAuthenticationHeaderEvent]);
//         plan.RequestEventsQueue.Enqueue([ContextEventNames.CheckAuthenticationStatusEvent]);
//     }
//
//     public void ExecutionPlanHasRequestData(IExecutionContext context, IExecutionPlanContract plan)
//     {
//         context.MetaData.Set(InzMetaDataKeys.Request.RawBodyString, plan.Body);
//         context.MetaData.Set(InzMetaDataKeys.Request.BodyType, plan.RequestDataType);
//         plan.RequestEventsQueue.Enqueue([ContextEventNames.DeserializeRequestDataEvent]);
//         if (plan.ValidateRequestData)
//         {
//             // TODO put context resources for request data validator event
//             // plan.RequestEventsQueue.Enqueue(new[] {EventNames.ValidateRequestDataEvent});
//         }
//     }
//
//     public void ExecutionPlanRequirePermissionsCheck(IExecutionContext context, IExecutionPlanContract plan)
//     {
//         plan.RequestEventsQueue.Enqueue([ContextEventNames.LoadUserPermissionsCacheEvent]);
//         context.MetaData.Set(InzMetaDataKeys.Request.PermissionsToCheck, plan.Permissions);
//         plan.RequestEventsQueue.Enqueue([ContextEventNames.CheckUserHasPermissionsEvent]);
//     }
//
//     public void ProcessExecutionContractTypeAttributes(IExecutionPlanContract contract, ICollection<object> attributes)
//     {
//         foreach (var attribute in attributes)
//         {
//             // TODO check that environment == EnvironmentModeEnum.Production in order to un-register the execution plan
//             if (attribute is ExecutionPlanTypeAttribute {ExecutionPlanType: ExecutionPlanType.Test} _)
//             {
//                 contract.IsRegistered = false;
//                 return;
//             }
//
//             if (attribute is ExecutorAttribute executor)
//             {
//                 contract.ExecutionGroup = executor.Group;
//                 contract.ExecutionLabel = executor.Plan;
//                 continue;
//             }
//
//             if (attribute is ApiEndpointAttribute endpoint)
//             {
//                 contract.UseRateLimiter = endpoint.RateLimited;
//                 contract.EndpointRoute = endpoint.Route;
//                 contract.EndpointMethod = endpoint.Method;
//                 continue;
//             }
//
//             if (attribute is MessagingQueueLabelAttribute queueLabel)
//             {
//                 contract.MessagingQueueRequestKey = queueLabel.Label;
//                 continue;
//             }
//
//             if (attribute is RequireAuthenticationAttribute _)
//             {
//                 contract.RequireAuthentication = true;
//                 continue;
//             }
//
//             if (attribute is RequirePermissionCheckAttribute permissions)
//             {
//                 contract.RequirePermissionCheck = true;
//                 contract.Permissions = permissions.Permissions.ToList();
//                 continue;
//             }
//
//             if (attribute is RequestDataTypeAttribute requestDataType)
//             {
//                 contract.HasRequestData = true;
//                 contract.RequestDataType = requestDataType.RequestDataType;
//                 contract.ValidateRequestData = requestDataType.ValidatorType is not null;
//                 if (contract.ValidateRequestData) contract.RequestDataValidatorType = requestDataType.RequestDataType;
//                 continue;
//             }
//
//             if (attribute is RequireConfigurationOptionsAttribute requireConfigurationOptions)
//             {
//                 contract.RequiredConfigurations = requireConfigurationOptions.Labels;
//                 continue;
//             }
//
//             if (attribute is PublishSystemNotificationsAttribute publishSystemNotifications)
//             {
//                 contract.SystemNotificationToPublish = publishSystemNotifications.Notifications;
//                 continue;
//             }
//
//             if (attribute is ResponseDataTypeAttribute responseDataType) contract.ResponseDataType = responseDataType.Type;
//         }
//     }
//
//     public void ProcessExecutionContractTypeInterfaces(IExecutionPlanContract planContract, ICollection<Type> interfaces)
//     {
//         planContract.ShouldRunBeforeDispatchingPreExecutionEventsTask = interfaces.Contains(typeof(IPreEventsExecutionContract));
//         planContract.ShouldRunExecutionTask = interfaces.Contains(typeof(IExecutionContract));
//         planContract.ShouldRunAfterDispatchingPostExecutionEventsTask = interfaces.Contains(typeof(IPostEventsExecutionContract));
//         planContract.HasEvents = interfaces.Contains(typeof(IExecutionEventsContract));
//     }
// }