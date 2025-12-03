namespace InzExecutionEvent.Resources;

// TODO: move all event keys into here
public static class InzEvents
{
    /// <summary>
    /// Pattern should be like so: e.execution.service.{entity}.{event-name}
    /// </summary>
    public static class ExecutionEvents
    {
        public const string TestEvent1 = "e.execution.service.test.1";
        public const string TestEvent2 = "e.execution.service.test.2";
        public const string TestEvent3 = "e.execution.service.test.3";
        public const string TestEvent4 = "e.execution.service.test.4";
    }

    /// <summary>
    /// Pattern should be like so: e.execution.{event-name}
    /// </summary>
    public static class ContextEvents
    {
        public const string DeserializeRequestDataEvent = "e.execution.deserialize-request-data";
        public const string DecodeAuthenticationHeaderEvent = "e.execution.decode-authentication-header";
        public const string CheckAuthenticationStatusEvent = "e.execution.check-is-authentication-status";
        public const string GenerateAuthenticationTokenEvent = "e.execution.generate-authentication-token";
        public const string ValidateRequestDataEvent = "e.execution.validate-request-data";
        public const string CheckUserHasPermissionsEvent = "e.execution.check-user-has-permissions";
        public const string LoadUserPermissionsCacheEvent = "e.execution.load-user-cached-permissions";
    }

    public static class RolesAndPermissions
    {
        public static class DataAccess
        {
            public const string GetAll = "e.data-access.roles-and-permissions.get-all";
        }
    }
}

public static class InzSystemNotifications
{
    public static class User
    {
        public const string UserLoggedIn = "system.notification.user.logged-in";
        public const string UserLoggedInHandler = "system.notification.handler.user.logged-in";
    }

    public static class Test
    {
        public const string TestOne = "system.notification.test";

        public const string Test1Handler = "system.notification.handler.test.1";
        public const string Test2Handler = "system.notification.handler.test.2";
        public const string Test3Handler = "system.notification.handler.test.3";
    }
}

public static class InzSystemNotificationHandlerNames
{
    public static class User
    {
        public const string UserLoggedIn = "system.notification.handler.user.logged-in";
    }

    public static class Test
    {
        public const string Test1 = "system.notification.handler.test.1";
        public const string Test2 = "system.notification.handler.test.2";
        public const string Test3 = "system.notification.handler.test.3";
    }
}