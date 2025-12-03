using InzExecutionEvent.Contracts.ExecutionContext;
using InzExecutionEvent.Contracts.ExecutionPlan;
using InzExecutionEvent.Resources;

namespace InzExecutionEvent.Utilities;

public static class ExecutionPlanUtility
{
    public static void RequiresAuthentication(IExecutionContext context, IExecutionPlanContract plan)
    {
        context.MetaData.Set(InzMetaDataKeys.Request.RawBodyString, plan.AuthenticationHeader);
        plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.DecodeAuthenticationHeaderEvent]);
        plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.CheckAuthenticationStatusEvent]);
    }

    public static void HasRequestData(IExecutionContext context, IExecutionPlanContract plan)
    {
        context.MetaData.Set(InzMetaDataKeys.Request.RawBodyString, plan.Body);
        context.MetaData.Set(InzMetaDataKeys.Request.BodyType, plan.RequestDataType);
        plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.DeserializeRequestDataEvent]);
        if (plan.ValidateRequestData)
        {
            // TODO: put context resources for request data validator event
            // plan.RequestEventsQueue.Enqueue(new[] {EventNames.ValidateRequestDataEvent});
        }
    }

    public static void RequirePermissionsCheck(IExecutionContext context, IExecutionPlanContract plan)
    {
        plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.LoadUserPermissionsCacheEvent]);
        context.MetaData.Set(InzMetaDataKeys.Request.PermissionsToCheck, plan.Permissions);
        plan.RequestEventsQueue.Enqueue([InzEvents.ContextEvents.CheckUserHasPermissionsEvent]);
    }
}