using InzExecutionComponents.Attributes;
using InzExecutionComponents.Contracts.ExecutionContext;
using InzExecutionComponents.Contracts.ExecutionPlan;
using InzExecutionComponents.Exception;
using InzExecutionComponents.ExecutionPlan;

namespace InzExecutionComponents.ExecutionContext;

internal sealed class ExecutionPlanContext(
    ExecutionPlanEngine executionPlanEngine,
    IExecutionPlanContract executionPlanContract,
    IExecutionPlanParametersContract? executionParametersContract
) : IInternalExecutionContext
{
    // private readonly ExecutionPlanEngine _executionPlanEngine = executionPlanEngine;
    // private readonly IExecutionPlanContract _executionPlanContract = executionPlanContract;
    // private readonly IExecutionParametersContract? _executionParametersContract = executionParametersContract;

    public string ExecutionPlanRegistrationKey { get; } = executionPlanContract.RegistrationKey;
    public IExecutionContextDataRepository MetaData { get; } = new ExecutionContextDataStore();
    public IExecutionContextDataRepository Store { get; } = new ExecutionContextDataStore();
    public IExecutionContextFailureRepository Failures { get; } = new ExecutionContextFailureRepository();

    public void SetInputData<T>(T data) where T : class, IExecutionPlanParametersContract
    {
        if (!executionPlanContract.HasInputData)
        {
            throw new InvalidOperationException(
                $"Attempting to set input data for execution plan [{executionPlanContract.Label}] that is not registered with an input data type, make sure to use [{nameof(ExecutionInputDataTypeAttribute<IExecutionPlanParametersContract>)}]."
            );
        }

        if (string.IsNullOrEmpty(executionPlanContract.InputDataKey)) throw new MissingContextKeyException(executionPlanContract.InputDataKey, executionPlanContract);
        Store.Set(executionPlanContract.InputDataKey, data);
    }

    public T GetInputData<T>() where T : class, IExecutionPlanParametersContract
    {
        if (!executionPlanContract.HasInputData)
        {
            throw new InvalidOperationException(
                $"Attempting to get input data for execution plan [{executionPlanContract.Label}] that is not registered with an input data type, make sure to use [{nameof(ExecutionInputDataTypeAttribute<IExecutionPlanParametersContract>)}]."
            );
        }

        if (string.IsNullOrEmpty(executionPlanContract.InputDataKey)) throw new MissingContextKeyException(executionPlanContract.InputDataKey, executionPlanContract);
        return Store.Get<T>(executionPlanContract.InputDataKey);
    }

    public void SetOutputData<T>(T data) where T : class, IExecutionPlanResultContract
    {
        if (!executionPlanContract.HasOutputData)
        {
            throw new InvalidOperationException(
                $"Attempting to set output data for execution plan [{executionPlanContract.Label}] that is not registered with an output data type, make sure to use [{nameof(ExecutionOutputDataTypeAttribute<IExecutionPlanResultContract>)}]."
            );
        }

        if (string.IsNullOrEmpty(executionPlanContract.OutputDataKey)) throw new MissingContextKeyException(executionPlanContract.OutputDataKey!, executionPlanContract);
        Store.Set(executionPlanContract.OutputDataKey, data);
    }

    public T GetOutputData<T>() where T : class, IExecutionPlanResultContract
    {
        if (!executionPlanContract.HasOutputData)
        {
            throw new InvalidOperationException(
                $"Attempting to get output data for execution plan [{executionPlanContract.Label}] that is not registered with an output data type, make sure to use [{nameof(ExecutionOutputDataTypeAttribute<IExecutionPlanResultContract>)}]."
            );
        }

        if (string.IsNullOrEmpty(executionPlanContract.OutputDataKey)) throw new MissingContextKeyException(executionPlanContract.OutputDataKey!, executionPlanContract);
        return Store.Get<T>(executionPlanContract.OutputDataKey);
    }
}