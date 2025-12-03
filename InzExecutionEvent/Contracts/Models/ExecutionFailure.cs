using InzExecutionEvent.Resources;

namespace InzExecutionEvent.Contracts.Models;

public class ExecutionFailure
{
    public bool Fatal { get; init; } // breaks execution
    public string Type { get; }
    public string Error { set; get; }
    public string? Code { get; }
    public string ExceptionMessage { get; }
    public string ExceptionStacktrace { get; }
    public string? InnerExceptionMessage { get; }
    public string? InnerExceptionStacktrace { get; }

    public ExecutionFailure(string code)
    {
        Code = code;
        Type = "un-resolved";
        Error = "un-resolved";
        ExceptionMessage = string.Empty;
        ExceptionStacktrace = string.Empty;
    }

    public ExecutionFailure(string type, string error, string code = "")
    {
        Type = type;
        Error = error;
        Code = string.IsNullOrEmpty(code) ? "unspecified" : code;
        ExceptionMessage = string.Empty;
        ExceptionStacktrace = string.Empty;
    }

    public ExecutionFailure(Exception ex)
    {
        Fatal = true;
        Type = ExecutionFailureTypes.Exception;
        Error = "failure";
        Code = "unspecified";
        ExceptionMessage = ex.Message;
        ExceptionStacktrace = ex.StackTrace ?? "not-found";
        if (ex.InnerException is null) return;
        InnerExceptionMessage = ex.InnerException.Message;
        InnerExceptionStacktrace = ex.InnerException.StackTrace;
    }

    // public ExecutionFailure(DataAccessFailure failure)
    // {
    //     Fatal = true;
    //     Type = ExecutionFailureTypes.DataAccess;
    //     Error = failure.Error;
    //     Code = failure.Code;
    //     ExceptionMessage = failure.ExceptionMessage;
    //     ExceptionStacktrace = failure.ExceptionStacktrace;
    //     InnerExceptionMessage = failure.InnerExceptionMessage;
    //     InnerExceptionStacktrace = failure.InnerExceptionStacktrace;
    // }
}

// TODO: Move into a separate file under Utilities directory in Common module

// TODO: Move into a separate file under Common module