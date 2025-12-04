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
}