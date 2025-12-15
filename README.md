# InzExecutionComponents

A comprehensive execution framework for defining, orchestrating, and executing complex business workflows with support for pre/post execution events,
notifications, and data flow between execution components. Designed for building event-driven, component-based applications
with separation of concerns.

## Overview

InzExecutionComponents is a flexible .NET 9.0 framework that enables developers to create maintainable and scalable business workflows. The
framework follows an event-driven architecture with clear separation between execution plans, events, and notifications.

### Key Features

- **Modular Architecture**: Component-based design with clear separation of concerns
- **Event-Driven**: Support for pre and post execution events with sequential and parallel execution
- **Declarative Configuration**: Attribute-based configuration system for easy setup
- **Asynchronous Execution**: Full async/await support throughout the execution pipeline
- **Context Management**: Rich execution context with data flow between components
- **Extensible Design**: Easy to extend with custom events, notifications, and execution contracts

## Architecture

The framework consists of three main engines that work together seamlessly:

### 1. Execution Plan Engine

Orchestrates execution plans, manages their lifecycle, and coordinates with other engines. Handles the core workflow execution including:

- Pre-execution event dispatching
- Main execution task execution
- Post-execution event dispatching
- Notification publishing
- Context data management

### 2. Execution Event Engine

Handles pre/post execution events with dependency checking and parallel execution capabilities. Events can run sequentially and/or in parallel
within the same queue.

### 3. Execution Notification Engine

Manages notifications and their handlers, enabling loose coupling between different parts of the system.

## Technology Stack

- **.NET 9.0**: Latest .NET runtime for optimal performance
- **Microsoft.Extensions.DependencyInjection**: Robust dependency injection framework
- **Microsoft.Extensions.Configuration.Abstractions**: Configuration management
- **Microsoft.Extensions.Configuration.Binder**: Configuration binding capabilities
- **C# 12.0**: Modern C# features including primary constructors and more

## Installation

To install the InzExecutionComponents package, add it to your project:

```xml
<!-- Add to your .csproj file -->
<PackageReference Include="InzExecutionComponents" Version="x.x.x"/>
```

Or via Package Manager:

```bash
dotnet add package InzExecutionComponents
```

## Getting Started

### Basic Setup

1. **Configure Services** in your Program.cs or Startup.cs:

```csharp
using InzExecutionComponents;
using InzExecutionComponents.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Register the execution components framework and scan for execution plans/events
builder.Services.AddInzExecutionComponents(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();

// Initialize the framework
app.Services.UseInzExecutionComponents();

// Get the execution manager to launch executions
var executionComponentManager = app.Services.GetRequiredService<IExecutionComponentManager>();

// Launch an execution plan
await executionComponentManager.LaunchExecution("MyExecutionPlan");
```

### Creating an Execution Plan

Define an execution plan using the `[ExecutionPlan]` attribute:

```csharp
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Contracts.Models;

[ExecutionPlan("Test")]  // Define execution plan
[ExecutionInputDataType<TestInputData>]  // Specify input data type
[ExecutionOutputDataType<TestOutputData>] // Specify output data type
[RegisterPreExecutionEvents(ExecutionEventKeys.PreEvent1)]  // Register pre-execution events
[RegisterPreExecutionEvents(ExecutionEventKeys.PreEvent2, ExecutionEventKeys.PreEvent3)]  // Parallel execution
[RegisterPostExecutionEvents(ExecutionEventKeys.PostEvent1)] // Register post-execution events
[PublishExecutionNotifications([ExecutionNotificationKeys.MyNotification])] // Publish notifications
public class TestExecutionPlan : IExecutionContract<IExecutionPlanResultContract>, 
                                 IPreEventsExecutionContract, 
                                 IPostEventsExecutionContract
{
    public async Task BeforeDispatchingPreExecutionEvents(IExecutionPlanContext context)
    {
        // Code to run before dispatching pre-execution events
        await Task.Delay(1000);
        Console.WriteLine("Before pre-execution events");
    }

    public async Task<ExecutionResult<IExecutionPlanResultContract>> Execute(IExecutionPlanContext context)
    {
        // Main execution logic here
        var inputData = context.GetInputData<TestInputData>();
        
        Console.WriteLine($"Processing: {inputData.SomeValue}");
        
        await Task.Delay(500); // Simulate work
        
        return new TestOutputData 
        {
            ProcessedValue = inputData.SomeValue + "_processed" 
        };
    }

    public async Task AfterDispatchingPostExecutionEvents(IExecutionPlanContext context)
    {
        // Code to run after post-execution events
        await Task.Delay(1000);
        Console.WriteLine("After post-execution events");
    }
}

// Define input data contract
public record TestInputData : IExecutionPlanParametersContract
{
    public string SomeValue { get; set; } = string.Empty;
}

// Define output data contract
public record TestOutputData : IExecutionPlanResultContract
{
    public string ProcessedValue { get; set; } = string.Empty;
}
```

### Creating Execution Events

Create events that can be triggered before or after execution plans:

```csharp
using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionEvent;

[ServiceExecutionEvent(ExecutionEventKeys.MyEvent, ["SomeRequiredKey"])] // Require specific context keys
public class MyExecutionEvent : IExecutionEvent
{
    public async Task Perform(IExecutionEventContext context)
    {
        Console.WriteLine("Executing my custom event");
        
        // Access data from context
        var value = context.Store.Get<string>("SomeRequiredKey");
        
        // Simulate work
        await Task.Delay(500);
        
        // Set data to context for other components
        context.Store.Set("EventResult", "Event completed successfully");
    }
}
```

### Using Context Store

The framework provides a context data stores to share data that flows between components during execution

```csharp
public async Task Execute(IExecutionPlanContext context)
{
    // Set data in context store
    context.Store.Set("myKey", "myValue");
    
    // Get data from context store
    var value = context.Store.Get<string>("myKey");
}
```

### Context Property Mapping with Attributes

Use `ExecutionContextStoreKeyAttribute` to automatically map properties from the execution plan input or output data to specific context store keys:

```csharp
public record TestInputData : IExecutionPlanParametersContract
{
    [ExecutionContextStoreKey("userId")]
    public string UserId { get; set; } = string.Empty;
    
    [ExecutionContextStoreKey("accountId")]
    public int AccountId { get; set; }
    
    public string NormalProperty { get; set; } = string.Empty; // Not mapped automatically
}
```

With this attribute, `UserId` will automatically be stored/retrieved using the "userId" key in the execution context store.

## Execution Flow

The typical execution flow follows this sequence:

1. **Before Pre-Events**: Execute `BeforeDispatchingPreExecutionEvents` if implemented
2. **Pre-Execution Events**: Dispatch registered pre-execution events
3. **Main Execution**: Execute the core `Execute` method
4. **Post-Execution Events**: Dispatch registered post-execution events
5. **After Post-Events**: Execute `AfterDispatchingPostExecutionEvents` if implemented
6. **Notification Publishing**: Publish any registered notifications

## Performance & Threading

- **Async/Await**: All operations are async/await based
- **Parallel Execution**: Events within the same queue execute in parallel using `Task.WhenAll()`
- **Sequential Queues**: Different event queues execute sequentially
- **Thread-Safe Context**: Uses `ConcurrentDictionary` for context stores to ensure thread safety
- **Optimized Registration**: Efficient assembly scanning and service registration during host build process

## Error Handling

The framework provides comprehensive error handling and failure management:

- **ExecutionPlanException**: Wraps exceptions occurring in execution plans
- **ExecutionEventException**: Wraps exceptions occurring in execution events
- **MissingContextKeyException**: Thrown when required context keys are missing

## Advanced Features

### Notification System
_To be defined later ..._

## Testing & Demos

The repository includes two demonstration applications:

- **Demo.DummyApp**: Simple example demonstrating basic framework features
- **Demo.FinancialTradingSystem**: Slightly complex example showcasing advanced use cases

## Best Practices

1. **Separate Concerns**: Each execution plan should focus on a single responsibility
2. **Use Events for Cross-Cutting Concerns**: Logging, validation, etc.
3. **Leverage Context for Data Flow**: Use context to pass data between components
4. **Handle Errors Gracefully**: Return proper execution results to indicate success/failure
5. **Use Attribute-Based Configuration**: Leverage attributes for declarative setup
6. **Design Immutable Data Contracts**: Use records for input/output data
7. **Consider Performance**: Use parallel execution queues when appropriate

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

To contribute:

1. Create a feature branch (`git checkout -b feature/amazing-feature`)
2. Commit your changes (`git commit -m 'Add amazing feature'`)
3. Push to the branch (`git push origin feature/amazing-feature`)
4. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

If you encounter any issues or have questions, please file an issue in the GitHub repository.
