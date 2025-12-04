using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent;

internal class InzExecutionEventBuilder(IServiceCollection serviceCollection, IConfiguration configuration) : IInzExecutionEventBuilder
{
    public IServiceCollection ServiceCollection { get; } = serviceCollection;
    public IConfiguration Configuration { get; } = configuration;
}