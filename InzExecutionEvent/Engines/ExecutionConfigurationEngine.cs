using System.Reflection;
using InzExecutionEvent.Attributes;
using InzExecutionEvent.Contracts.Configuration;
using InzExecutionEvent.Contracts.ExecutionContext;
using Microsoft.Extensions.Configuration;

namespace InzExecutionEvent.Engines;

public class ExecutionConfigurationEngine
{
    private Dictionary<string, IExecutionConfigurationOptions> ConfigurationOptionsMap { get; } = new();

    public void RegisterConfigurationsFromAssembly(Assembly assembly, IConfiguration configuration)
    {
        var types = assembly.GetTypes().Where(t => t.GetCustomAttributes().Any(a => a is RegisterExecutionConfigOptionsAttribute)).ToList();
        foreach (var type in types)
        {
            var a = type.GetCustomAttributes().First(a => a is RegisterExecutionConfigOptionsAttribute);
            if (a is not RegisterExecutionConfigOptionsAttribute attribute)
            {
                Console.WriteLine($"Skipping configuration class [{type.Name}] because applied attribute is not of type [ConfigurationOptionsAttribute]");
                continue;
            }

            var instance = Activator.CreateInstance(type);
            if (instance is null)
            {
                Console.WriteLine($"Skipping configuration labeled [{attribute.Label}]");
                continue;
            }

            configuration.GetSection(attribute.Label).Bind(instance);
            ConfigurationOptionsMap.Add(attribute.Label, (instance as IExecutionConfigurationOptions)!);
        }
    }

    public void LoadConfigurationOptionsIntoContext(IExecutionContext context, string[] labels)
    {
        foreach (var label in labels)
        {
            if (ConfigurationOptionsMap.TryGetValue(label, out var configuration)) configuration.LoadIntoContextMetadata(context);
            else throw new Exception($"Configuration with label {label} was not found");
        }
    }
}