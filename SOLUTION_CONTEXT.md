# Solution: InzExecutionComponents

## 1. High-Level Architecture

- **Concept:** This solution provides a comprehensive execution framework for defining, orchestrating, and executing complex business workflows with
  support for pre/post execution events, notifications, configuration management, and data flow between execution components. It's designed for
  building event-driven, component-based applications with strong separation of concerns.

- **Project Graph:**

```
InzExecutionComponents (Core Library)
├── Demo.DummyApp (Consumer/Demo App)
└── Demo.FinancialTradingSystem (Consumer/Demo App)
```

- **Tech Stack:**
    - .NET 9.0
    - Microsoft.Extensions.DependencyInjection (9.0.0)
    - Microsoft.Extensions.Configuration.Abstractions (9.0.0)
    - Microsoft.Extensions.Configuration.Binder (9.0.0)
    - Web API for demo applications

## 2. Core Library Deep Dive (InzExecutionComponents)

- **Public API Surface:**
    - `IExecutionComponentManager`: Main entry point for launching execution plans with `LaunchExecution` methods
    - `IExecutionPlanContext`: Context interface for execution plan operations with input/output data access
    - `IExecutionEventContext`: Context interface for execution event operations
    - `IExecutionEvent`: Interface for implementing execution events
    - `IExecutionContract<T>`: Interface for execution logic implementation
    - `IPreEventsExecutionContract`: Interface for pre-execution tasks
    - `IPostEventsExecutionContract`: Interface for post-execution tasks
    - `IExecutionPlanParametersContract`: Marker interface for execution input data
    - `IExecutionPlanResultContract`: Marker interface for execution output data
    - `IExecutionNotificationHandler`: Interface for notification handlers
    - Extension methods: `AddInzExecutionComponents()` and `UseInzExecutionComponents()` for DI setup
    - Rich attribute-based configuration system including:
        - `ExecutionPlanAttribute` to define execution plans with name, group, and type
        - `ExecutionContextStoreKeyAttribute` to map individual properties of input/output data contracts to specific context store keys, enabling granular access to properties across the execution pipeline
        - `ExecutionInputDataTypeAttribute<T>` and `ExecutionOutputDataTypeAttribute<T>` for defining input/output data contracts
        - `RegisterPreExecutionEvents` and `RegisterPostExecutionEvents` attributes for event orchestration
        - `PublishExecutionNotifications` attribute for publishing notifications from execution plans
        - `ExecutionConfigurationOptionsAttribute` for configuration dependency management
        - `MessagingQueueLabelAttribute` for messaging system integration

- **Internal Logic:** The framework uses three main engines that work together:
    - `ExecutionPlanEngine`: Orchestrates execution plans, manages their lifecycle, and coordinates with other engines
    - `ExecutionEventEngine`: Handles pre/post execution events with dependency checking and parallel execution
    - `ExecutionNotificationEngine`: Manages notifications and their handlers
    - `ExecutionConfigurationEngine`: Handles configuration options loading into execution context
    - Notification and handler architecture: `IExecutionNotificationHandler` interface with `ExecutionNotificationHandlerAttribute` mapping handlers to notifications and relationships between `ExecutionNotificationContract` and `ExecutionNotificationHandlerContract`

- **Design Patterns:**
    - Dependency Injection with Service Locator pattern
    - Strategy pattern (for different execution implementations)
    - Command pattern (for individual execution events)
    - Observer pattern (for notifications)
    - Repository pattern (for context data stores)
    - Fluent API (for DI extension methods)
    - Registry pattern (for managing execution plans, events, and notifications)
    - Assembly Scouting system: The `Scouters` class discovers execution plans, events, notifications, and handlers through reflection using methods like `ExecutionPlanTypes()`, `ExecutionEventTypes()`, `ExecutionNotificationTypes()` that identify components with specific attributes
    - Attribute Processing Mechanism: The `ExecutionPlanUtility` class processes various attributes using reflection to extract metadata and configure execution plan behavior. This includes processing `ExecutionContextStoreKeyAttribute` on data contract properties, `ExecutionConfigurationOptionsAttribute` for configuration dependencies, `PublishExecutionNotificationsAttribute` for notification publishing, and attributes that define pre/post execution event dependencies. The utility methods `ProcessPlanInputDataDetails()` and `ProcessPlanOutputDataDetails()` specifically handle `ExecutionContextStoreKeyAttribute` to extract property details and create mappings between property values and context store keys

- **Configuration:** Currently reads configuration from IConfiguration for execution configuration options with
  `RegisterExecutionConfigOptionsAttribute`. Configuration loading is currently disabled in the main engine (commented out).
  Future feature: The `ExecutionConfigurationEngine` is implemented but not enabled in the main pipeline, and configuration options are read from `IConfiguration` using `GetSection().Bind()` pattern.

- **Extension Methods:**
    - `AddInzExecutionComponents(IServiceCollection, IConfiguration, params Assembly[])` registers all framework services and scans assemblies for
      execution plans, events, and notifications
    - `UseInzExecutionComponents(IServiceProvider)` starts all engines and makes them ready for execution

## 3. Implementation Details & "Gotchas"

- **Error Handling:**
    - Exceptions in execution plans are wrapped in `ExecutionPlanException`
    - Exceptions in events are wrapped in `ExecutionEventException`
    - Framework includes a Failure repository that supports both regular and fatal failures
    - Missing context keys throw `MissingContextKeyException`
    - The framework has a failure processing system that can halt execution based on fatal errors
    - Detailed `ExecutionFailure` model with properties for `Fatal`, `Type`, `Error`, `Code`, and exception details
    - Failures are categorized as fatal (which stops execution) vs non-fatal (which allows continuation)
    - `ExecutionContextFailureRepository` tracks failures and provides `HasFatal()` method to control execution flow

- **Threading/Async:**
    - All execution operations are async/await based
    - Events within the same array are executed in parallel using `Task.WhenAll()`
    - Each event queue is processed sequentially
    - The framework properly handles async operations through the entire execution pipeline

- **Interface Hierarchy:**
    - Interface inheritance relationships: `IExecutionPlanContext` extends `IInternalExecutionContext`, and the relationship between `IExecutionRegistryContract` and execution contracts

- **Data Access:**
    - No database access in the core library - uses in-memory execution context stores
    - `ExecutionContextDataStore` serves as a key-value store for data passed between events
    - Context includes both a Store (for execution data) and MetaData (for configuration/metadata)
    - `ExecutionPlanContext` provides typed access to input/output data
    - Execution plan data flow mechanism: `IExecutionPlanParametersContract` and `IExecutionPlanResultContract` define data contracts, and the context store mechanism transfers input/output data between execution components using `SetInputData<T>()`, `GetInputData<T>()`, `SetOutputData<T>()`, and `GetOutputData<T>()` methods
    - `ExecutionContextStoreKeyAttribute` functionality: Properties in data contracts decorated with this attribute are automatically mapped to specific context store keys during execution plan processing. The `ExecutionPlanUtility` processes these attributes and extracts property details, storing them in `InputDataPropertiesDetailsForContextStore` and `OutputDataPropertiesDetailsForContextStore` dictionaries. During execution, the `LoadInputValuesIntoExecutionContextStore` and `LoadOutputValuesIntoExecutionContextStore` methods in `ExecutionPlanEngine` automatically extract property values and store them in the context using the specified keys, making individual properties accessible to events and other components in the execution pipeline

- **Context Data Repository Architecture:**
    - Dual-context system: `MetaData` repository for configuration/metadata and `Store` repository for execution data
    - `ExecutionContextDataStore` uses `ConcurrentDictionary<string, object>` for thread-safe access
    - The `Check()` method validates required keys exist in context repositories before execution

## 4. Developer Guide

- **Build Instructions:**
    - `dotnet build` - Build the entire solution
    - `dotnet build InzExecutionComponents.sln` - Build solution specifically
    - `dotnet test` - Run tests (though no test projects were found in this solution)

- **Running Locally:**
    - Both Demo.DummyApp and Demo.FinancialTradingSystem can be set as startup projects
    - Use `dotnet run --project Demo.DummyApp` to run the simple demo
    - Use `dotnet run --project Demo.FinancialTradingSystem` to run the financial trading demo
    - The framework is initialized in Program.cs using the extension methods

- **Key Files map:**
    - `/InzExecutionComponents/InzExecutionComponents.cs` - Main extension methods for DI registration
    - `/InzExecutionComponents/ExecutionComponentManager.cs` - Core execution manager implementation
    - `/InzExecutionComponents/ExecutionPlan/ExecutionPlanEngine.cs` - Core orchestration engine
    - `/InzExecutionComponents/ExecutionEvent/ExecutionEventEngine.cs` - Event orchestration engine
    - `/InzExecutionComponents/ExecutionNotification/ExecutionNotificationEngine.cs` - Notification engine
    - `/InzExecutionComponents/ExecutionContext/ExecutionPlanContext.cs` - Execution context implementation
    - `/Demo.DummyApp/ExecutionPlans/TestExecutionPlan.cs` - Example of how to implement execution plans
    - `/Demo.FinancialTradingSystem/ExecutionPlans/AlgorithmicTradingExecutionPlan.cs` - Complex example with multiple data models