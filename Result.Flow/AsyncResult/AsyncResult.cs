using System.Runtime.CompilerServices;
using Result.Flow.Result;

namespace Result.Flow.AsyncResult;

[AsyncMethodBuilder(typeof(AsyncResultMethodBuilder<>))]
public readonly struct AsyncResult<T>
{
    private readonly Task<Result<T>> _task = Task.FromResult<Result<T>>(default);

    public async Task<bool> IsSuccess()
    {
        var result = await _task;
        return result.IsSuccess;
    }

    public AsyncResult(Task<Result<T>> task) => _task = task;

    public Task<Result<T>> AsTask() => _task;

    public ResultAwaiter<T> GetAwaiter() => new(this);

    public async Task Match(Action<T> leftFunc, Action<Error> rightFunc)
    {
        var result = await _task;
        result.Match(leftFunc, rightFunc);
    }

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

    public static implicit operator AsyncResult<T>(Task<Result<T>> task) => new(task);
    
    public static AsyncResult<T> Ok(T value) => new(Task.FromResult(new Result<T>(value)));

    public static AsyncResult<T> Fail(Error error) => new(Task.FromResult(new Result<T>(error)));
}

public readonly struct ResultAwaiter<T> : INotifyCompletion
{
    private readonly Task<Result<T>> _task;

    public ResultAwaiter(AsyncResult<T> asyncResult)
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

public sealed class AsyncResultMethodBuilder<T>
{
    public static AsyncResultMethodBuilder<T> Create() => new();

    public AsyncResult<T> Task { get; private set; }

    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }

    public void SetException(Exception exception) { }

    //public void SetResult(T result) => Task = AsyncResult.Ok(result);

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
        => GenericAwaitOnCompleted(ref awaiter, ref stateMachine);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
        => GenericAwaitOnCompleted(ref awaiter, ref stateMachine);

    private void GenericAwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }
}