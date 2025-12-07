using InzExecutionEvent.Contracts.ExecutionContext;

namespace InzExecutionEvent.ExecutionContext;

internal static class ExecutionContextStaticEngine
{
    public static ISystemExecutionContext Build(IServiceProvider provider)
    {
        return new CoreExecutionContext
        {
            Store = new ExecutionContextDataStore(),
            Failures = new ExecutionContextFailureRepository(),
            Results = new ExecutionContextResultRepository(),
            // Response = new ExecutionResponse(),
            ServiceProvider = provider
        };
    }

    public static bool CheckAndProcessFailures(ISystemExecutionContext context)
    {
        if (!context.Failures.HasFailures()) return false;
        context.Response.Successful = !context.Failures.HasFatal();
        context.Response.Errors = context.Failures.Failures();
        return context.Failures.HasFatal();
    }

    // public static void ProcessExecutionResult(ISystemExecutionContext context)
    // {
    //     if (CheckAndProcessFailures(context)) return;
    //     context.Response = new ExecutionGroupResponse
    //     {
    //         Successful = !context.Failures.HasFatal(),
    //         Errors = context.Failures.Failures()
    //     };
    //
    //     if (context.Results.Count() == 0) return;
    //     // var results = context.Results.Results();
    //     // .GroupBy(result => result.Label)
    //     // .ToDictionary(g => g.Key, g => g.Select(r => r.Result));
    //     // context.Response.Results = results;
    //     context.Response.Result = context.Results.Results().FirstOrDefault();
    // }
    //
    // public static void LoadMetadataFromHttpRequest(ISystemExecutionContext context, HttpRequest request)
    // {
    //     var metadataStore = new ExecutionContextMetaDataStore();
    //     foreach (var (key, value) in request.Query)
    //     {
    //         if (Guid.TryParse(value.ToString(), out var id)) metadataStore.Set(FormatHttpRequestQueryMetaDataKey(key), id);
    //         else metadataStore.Set(key, value.ToString());
    //         metadataStore.Set(FormatHttpRequestQueryMetaDataKey(key), value.ToString());
    //     }
    //
    //     foreach (var (key, value) in request.RouteValues)
    //     {
    //         if (value is null) continue;
    //         if (value is Guid id) metadataStore.Set(key, id);
    //         else metadataStore.Set(key, value);
    //     }
    //
    //     foreach (var (key, value) in request.Headers)
    //     {
    //         if (metadataStore.Has(FormatHttpRequestHeaderMetaDataKey(key))) Console.WriteLine($"Skipping header with key {key} because it already exists in the metadata, value is {value}");
    //         if (Guid.TryParse(value.ToString(), out var id)) metadataStore.Set(FormatHttpRequestHeaderMetaDataKey(key), id);
    //         else metadataStore.Set(FormatHttpRequestHeaderMetaDataKey(key), value.ToString());
    //     }
    //
    //     context.MetaData = metadataStore;
    // }

    // public static string FormatHttpRequestQueryMetaDataKey(string label) => $"req.query.{label}";
    // public static string FormatHttpRequestHeaderMetaDataKey(string label) => $"req.header.{label}";
}