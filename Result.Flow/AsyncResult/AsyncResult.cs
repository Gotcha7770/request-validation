using System.Runtime.CompilerServices;
using Result.Flow.Interfaces;
using Result.Flow.Result;

namespace Result.Flow.AsyncResult;

public readonly struct ResultAwaiter<T> : INotifyCompletion
{
    private readonly Task<Result<T>> _task;

    public ResultAwaiter(IAsyncResult<T> asyncResult)
    {
        _task = asyncResult.AsTask();
    }

    public bool IsCompleted => _task.IsCompleted;

    public void OnCompleted(Action continuation)
    {
        _task.GetAwaiter().OnCompleted(continuation);
    }

    public Result<T> GetResult() => _task.Result;
}

public class AsyncResult<T> : IAsyncResult<T>
{
    private readonly Task<Result<T>> _task;

    public async Task<bool> IsSuccess()
    {
        var result = await _task;
        return result.IsLeft;
    }

    public AsyncResult(Task<Result<T>> task) => _task = task;
    
    public async Task<R> Match<R>(Func<T, R> leftFunc, Func<Error, R> rightFunc)
    {
        var result = await _task;
        return result.Match(leftFunc, rightFunc);
    }

    public async Task<R> Match<R>(Func<T, Task<R>> leftFunc, Func<Error, Task<R>> rightFunc)
    {
        var result = await _task;
        return await result.Match(leftFunc, rightFunc);
    }

    public Task<Result<T>> AsTask() => _task;

    public ResultAwaiter<T> GetAwaiter() => new ResultAwaiter<T>(this);

    public async Task Match(Action<T> leftFunc, Action<Error> rightFunc)
    {
        var result = await _task;
        result.Match(leftFunc, rightFunc);
    }

    public static implicit operator AsyncResult<T>(Task<Result<T>> task) => new AsyncResult<T>(task);
}

public class AsyncResult : AsyncResult<Unit>
{
    public static AsyncResult Ok() => new AsyncResult(Task.FromResult(new Result<Unit>(Unit.Value)));
    
    public static AsyncResult<T> Ok<T>(T value) => new AsyncResult<T>(Task.FromResult(new Result<T>(value)));
    public static AsyncResult<T> Fail<T>(Error error) => new AsyncResult<T>(Task.FromResult(new Result<T>(error)));

    public AsyncResult(Task<Result<Unit>> task) : base(task) { }
}