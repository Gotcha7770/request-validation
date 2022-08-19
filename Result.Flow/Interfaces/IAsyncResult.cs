using Result.Flow.AsyncResult;
using Result.Flow.Result;

namespace Result.Flow.Interfaces;

public interface IAsyncResult<T>
{
    Task<bool> IsSuccess();
    Task Match(Action<T> leftFunc, Action<Error> rightFunc);
    Task<R> Match<R>(Func<T, R> leftFunc, Func<Error, R> rightFunc);
    Task<R> Match<R>(Func<T, Task<R>> leftFunc, Func<Error, Task<R>> rightFunc);
    Task<Result<T>> AsTask();
    ResultAwaiter<T> GetAwaiter();
}