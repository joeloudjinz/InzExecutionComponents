namespace InzExecutionEvent.Resources;

public static class InzMetaDataKeys
{
    public static class Request
    {
        public const string RawBodyString = "ctx.metadata.request.body.value";
        public const string BodyType = "ctx.metadata.request.body.type";
        public const string DeserializedBody = "ctx.metadata.request.deserialized-body";
        public const string PermissionsToCheck = "ctx.metadata.request.permissions-to-check";
    }

    public static class AuthenticatedUser
    {
        public const string Id = "ctx.metadata.authentication.user-id";
        public const string ActiveOrganizationId = "ctx.metadata.authentication.user.active-organization-id";
        public const string ActiveLocationId = "ctx.metadata.authentication.user.active-location-id";
        public const string HasMfaEnabled = "ctx.metadata.authentication.user.has-mfa-enabled";
        public const string PermissionsCacheService = "ctx.metadata.authenticated-user.permissions-cache-service";
    }

    public static class AuthenticationConfigs
    {
        // public const string RawString = "ctx.metadata.request.headers.authentication.value";
        public const string SecretKey = "ctx.metadata.config.jwt.secret";
        public const string LifetimeValue = "ctx.metadata.config.jwt.lifetime";
        public const string TokenLength = "ctx.metadata.config.jwt.length";
        public const string TokenExpirationTime = "ctx.metadata.config.jwt.expiration-time";
        // public const string DecodedModel = "ctx.metadata.request.authentication-header.model";
        // public const string IsValid = "ctx.metadata.authentication.is-valid";
        // public const string IsExpired = "ctx.metadata.authentication.is-expired";
        // public const string GeneratedToken = "ctx.metadata.authentication.generated-token";
        public const string RequireValidation = "ctx.metadata.config.authentication.require-validation";
        public const string PasswordSecret = "ctx.metadata.config.password-secret";
    }

    public static class ServerGlobalConfigs
    {
        public const string Environment = "ctx.metadata.config.server.global.environment";
        public const string UseOverridableTestMode = "ctx.metadata.config.server.global.use-overridable-test-mode";

    }
}