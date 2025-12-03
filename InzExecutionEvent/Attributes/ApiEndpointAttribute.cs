using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ApiEndpointAttribute(ApiEndPointMethod method, string route, bool rateLimited = false) : Attribute
{
    public HttpMethod Method { get; } = method switch
    {
        ApiEndPointMethod.Get => HttpMethod.Get,
        ApiEndPointMethod.Post => HttpMethod.Post,
        ApiEndPointMethod.Put => HttpMethod.Put,
        ApiEndPointMethod.Patch => HttpMethod.Patch,
        ApiEndPointMethod.Delete => HttpMethod.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };

    public string Route { get; } = route;
    public bool RateLimited { get; } = rateLimited;
}