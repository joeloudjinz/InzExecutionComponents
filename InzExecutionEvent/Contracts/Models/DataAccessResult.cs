// namespace InzExecutionEvent.Contracts.Models;
//
// public class DataAccessResult<TValue>
// {
//     private readonly TValue? _value;
//     private readonly DataAccessFailure? _error;
//
//     public TValue? Result => _value;
//     public DataAccessFailure? Failure => _error;
//     
//     public bool IsError { get; }
//     public bool IsSuccess => !IsError;
//
//     private DataAccessResult(DataAccessFailure error)
//     {
//         IsError = false;
//         _error = error;
//     }
//
//     private DataAccessResult(TValue value)
//     {
//         IsError = false;
//         _value = value;
//     }
//
//     public static implicit operator DataAccessResult<TValue>(TValue value) => new(value);
//     public static implicit operator DataAccessResult<TValue>(DataAccessFailure error) => new(error);
//
//     // public TResult Match<TResult>(Func<TValue, TResult> success, Func<DataAccessFailure, TResult> failure) => !IsError ? success(_value!) : failure(_error!);
// }