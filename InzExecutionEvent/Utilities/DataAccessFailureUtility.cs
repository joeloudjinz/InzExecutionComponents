// using InzExecutionEvent.Contracts.Models;
// using InzExecutionEvent.Resources;
//
// namespace InzExecutionEvent.Utilities;
//
// public static class DataAccessFailureUtility
// {
//     public static DataAccessFailure CodeFailure(string code)
//     {
//         return new DataAccessFailure(code);
//     }
//
//     public static DataAccessFailure ValidationFailure(string error)
//     {
//         return new DataAccessFailure(DataAccessFailureTypes.Validation, error);
//     }
//
//     public static DataAccessFailure FromException(Exception ex)
//     {
//         return new DataAccessFailure(ex);
//     }
// }