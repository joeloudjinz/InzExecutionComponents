using InzExecutionEvent.Enums;

namespace InzExecutionEvent.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RequestDataTypeAttribute : Attribute
{
    public RequestDataTypeAttribute(
        Type requestDataType,
        Type validatorType,
        RequestDataPropertiesCasing casing = RequestDataPropertiesCasing.CamelCase
    )
    {
        RequestDataType = requestDataType;
        ValidatorType = validatorType;
        PropertiesCasing = casing;
    }
        
    public RequestDataTypeAttribute(
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