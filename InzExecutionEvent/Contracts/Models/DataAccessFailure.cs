// using InzExecutionEvent.Resources;
//
// namespace InzExecutionEvent.Contracts.Models;
//
// public class DataAccessFailure
// {
//     public string Type { get; }
//     public string Error { get; }
//     public string? Code { get; }
//     public string ExceptionMessage { get; }
//     public string ExceptionStacktrace { get; }
//     public string? InnerExceptionMessage { get; }
//     public string? InnerExceptionStacktrace { get; }
//
//     public DataAccessFailure(string code)
//     {
//         Code = code;
//         Type = "un-resolved";
//         Error = "un-resolved";
//         ExceptionMessage = string.Empty;
//         ExceptionStacktrace = string.Empty;
//     }
//
//     public DataAccessFailure(string type, string error, string code = "")
//     {
//         Type = type;
//         Error = error;
//         Code = string.IsNullOrEmpty(code) ? "unspecified" : code;
//         ExceptionMessage = string.Empty;
//         ExceptionStacktrace = string.Empty;
//     }
//
//     public DataAccessFailure(Exception ex)
//     {
//         Type = DataAccessFailureTypes.Exception;
//         Error = "failure";
//         Code = "unspecified";
//         ExceptionMessage = ex.Message;
//         ExceptionStacktrace = ex.StackTrace ?? "not-found";
//         if (ex.InnerException is null) return;
//         InnerExceptionMessage = ex.InnerException.Message;
//         InnerExceptionStacktrace = ex.InnerException.StackTrace;
//     }
// }