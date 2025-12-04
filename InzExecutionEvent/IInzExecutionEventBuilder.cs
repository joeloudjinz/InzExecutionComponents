using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InzExecutionEvent;

public interface IInzExecutionEventBuilder
{
    public IServiceCollection ServiceCollection { get; }
    public IConfiguration Configuration { get; }
}