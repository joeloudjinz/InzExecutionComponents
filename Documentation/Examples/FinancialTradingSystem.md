# Automated Financial Trading System with Risk Management

## Overview
This example demonstrates a complex financial trading platform that executes algorithmic trades while managing risk across multiple markets and asset classes. It showcases the InzExecutionComponents framework's capability to handle sophisticated business processes with strict requirements for data integrity, regulatory compliance, and performance.

## Execution Plan Configuration

### Class Definition
```csharp
[ExecutionPlan(ExecutionPlanKeys.FinancialTradingAlgo)]
[MessagingQueueLabel(MessagingQueueLabels.TradingAlgoQueue)]
[ExecutionInputDataType(typeof(TradingInputData))]
[ExecutionOutputDataType(typeof(TradingOutputData))]
[ExecutionConfigurationOptions([ConfigurationLabels.TradingRiskLimits])]
[PublishExecutionNotifications([NotificationKeys.TradeExecution, NotificationKeys.RiskAlert])]
[RegisterPreExecutionEvents(ExecutionEventKeys.MarketDataValidation)]
[RegisterPreExecutionEvents(ExecutionEventKeys.LiquidityAssessment, ExecutionEventKeys.RegulatoryCompliance, ExecutionEventKeys.PositionOverlapCheck)]
[RegisterPostExecutionEvents(ExecutionEventKeys.TradeLogging, ExecutionEventKeys.PerformanceMetricsUpdate)]
public class AlgorithmicTradingExecutionPlan :
    IExecutionContract<TradingOutputData>,
    IPreEventsExecutionContract,
    IPostEventsExecutionContract
```

### Input and Output
- **Input**: Market data, portfolio positions, risk thresholds, trading signals
- **Output**: Trade execution results, position updates, risk metrics

## Implementation Details

### Before Pre-Execution Events
```csharp
public async Task BeforeDispatchingPreExecutionEvents(IExecutionContext context)
{
    // Initialize trading context with market data
    var marketData = context.Store.Get<MarketData>(ContextStoreKeys.MarketData);
    context.Store.Set(ContextStoreKeys.ExecutionId, Guid.NewGuid().ToString());
}
```

### Main Execution Logic
```csharp
public async Task<ExecutionResult<TradingOutputData>> Execute(IExecutionContext context)
{
    var inputData = context.Store.Get<TradingInputData>(ContextStoreKeys.TradingInput);
    // Execute trading algorithms
    var result = PerformAlgorithmicTrading(inputData);
    return result;
}
```

### After Post-Execution Events
```csharp
public async Task AfterDispatchingPostExecutionEvents(IExecutionContext context)
{
    // Cleanup and finalize execution metrics
    var executionId = context.Store.Get<string>(ContextStoreKeys.ExecutionId);
    context.Results.Set(executionId, ExecutionResultValues.Completed);
}
```

## Pre-Execution Events

### Sequential Event
- `MarketDataValidationEvent` - Validates incoming market data integrity
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.MarketDataValidation,
      requiredStoreKeys: [ContextStoreKeys.MarketData],
      requiredConfigurations: [ConfigurationLabels.ValidationRules],
      inputType: typeof(MarketData),
      outputType: typeof(ValidationResult))]
  ```

### Parallel Events
The following events execute in parallel as an array in pre-execution registration:

- `LiquidityAssessmentEvent` - Evaluates market liquidity for planned trades
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.LiquidityAssessment,
      requiredStoreKeys: [ContextStoreKeys.MarketData, ContextStoreKeys.TradePlan])]
  ```
  
- `RegulatoryComplianceEvent` - Ensures trades comply with regulations
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.RegulatoryCompliance,
      requiredStoreKeys: [ContextStoreKeys.TradePlan, ContextStoreKeys.ComplianceRules])]
  ```

- `PositionOverlapCheckEvent` - Identifies potential conflicts with existing positions
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.PositionOverlapCheck,
      requiredStoreKeys: [ContextStoreKeys.CurrentPositions, ContextStoreKeys.NewTrade])]
  ```

## Post-Execution Events

- `TradeLoggingEvent` - Records all executed trades to audit trail
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.TradeLogging,
      requiredStoreKeys: [ContextStoreKeys.TradeResults])]
  ```

- `PortfolioRebalancingEvent` - Adjusts positions based on new executions
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.PortfolioRebalancing,
      requiredStoreKeys: [ContextStoreKeys.TradeResults, ContextStoreKeys.CurrentPositions])]
  ```

- `PerformanceMetricsUpdateEvent` - Updates strategy performance indicators
  ```csharp
  [ServiceExecutionEvent(ExecutionEventKeys.PerformanceMetricsUpdate,
      requiredStoreKeys: [ContextStoreKeys.ExecutionMetrics])]
  ```

## Notifications

The system publishes two types of notifications:

- `TradeExecutionNotification` - Sent to monitoring dashboard and compliance team
  ```csharp
  [ExecutionNotification(NotificationKeys.TradeExecution)]
  ```

- `RiskAlertNotification` - Triggered if risk thresholds exceeded
  ```csharp
  [ExecutionNotification(NotificationKeys.RiskAlert)]
  ```

Handlers for each notification type process the business logic accordingly.

## Framework Features Showcased

This example highlights several key features of the InzExecutionComponents framework:

1. **Attribute-based configuration**: Complex multi-stage processes are defined declaratively using attributes
2. **Parallel execution**: Efficiency is improved through array notation for concurrent event execution
3. **Typed input/output**: Type safety is enforced with validation using ExecutionInputDataType/OutputDataType
4. **Configuration binding**: ExecutionConfigurationOptions enable flexible configuration management
5. **Messaging queue integration**: MessagingQueueLabel facilitates distributed processing
6. **Execution context**: State is maintained between events to preserve portfolio information
7. **Comprehensive notification system**: Multiple handlers can process each notification type
8. **Failure handling**: Robust context management ensures proper error recovery
9. **Real-time processing**: The system handles live market data with appropriate latency considerations