namespace InzExecutionComponents.Resources;

public static class InzEvents
{
    public static class ContextEvents
    {
        public const string DeserializeRequestDataEvent = "e.execution.deserialize-request-data";
        public const string CheckUserHasPermissionsEvent = "e.execution.check-user-has-permissions";
        public const string LoadUserPermissionsCacheEvent = "e.execution.load-user-cached-permissions";
    }
}