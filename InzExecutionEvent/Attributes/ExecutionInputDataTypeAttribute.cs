using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionInputDataTypeAttribute : Attribute
{
    public ExecutionInputDataTypeAttribute(
        Type requestDataType,
        Type validatorType,
        RequestDataPropertiesCasing casing = RequestDataPropertiesCasing.CamelCase
    )
    {
        RequestDataType = requestDataType;
        ValidatorType = validatorType;
        PropertiesCasing = casing;
    }

    public ExecutionInputDataTypeAttribute(
        Type requestDataType,
        RequestDataPropertiesCasing casing = RequestDataPropertiesCasing.CamelCase
    )
    {
        RequestDataType = requestDataType;
        ValidatorType = null;
        PropertiesCasing = casing;
    }

    public Type RequestDataType { get; }
    public Type? ValidatorType { get; }
    public RequestDataPropertiesCasing PropertiesCasing { get; }
}