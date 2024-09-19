#nullable enable
using Result.Flow.Result;

namespace Result.Flow.AsyncResult;

public static partial class AsyncResultExtensions
{
    public static AsyncResult<R> Select<T, R>(this AsyncResult<T> source, Func<T, R> selector)
    {
        var task = source.Match(
            x => Result<R>.Ok(selector(x)), 
            Result<R>.Fail);
        return new AsyncResult<R>(task);
    }
    
    public static AsyncResult<R> SelectMany<T, R>(this AsyncResult<T> source, Func<T, AsyncResult<R>> selector)
    {
        var task = source.Match(
            x => selector(x).AsTask(), 
            e => Task.FromResult(Result<R>.Fail(e)));

        return new AsyncResult<R>(task);
    }

    public static AsyncResult<R2> SelectMany<T, R1, R2>(
        this AsyncResult<T> source,
        Func<T, AsyncResult<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        var task = source.Match(
            x => selector(x).Select(y => resultSelector(x, y)).AsTask(),
            e => Task.FromResult(Result<R2>.Fail(e)));
        
        return new AsyncResult<R2>(task);
    }

    // class SelectManyAsyncResult<T, R> : IAsyncResult<R>
    // {
    //     private readonly IAsyncResult<T> _source;
    //     private readonly Func<T, IAsyncResult<R>> _selector;
    //
    //     public SelectManyAsyncResult(IAsyncResult<T> source, Func<T, IAsyncResult<R>> selector)
    //     {
    //         _source = source;
    //         _selector = selector;
    //     }
    //
    //     public Task<bool> IsSuccess() => _source.IsSuccess();
    //     
    //     public async Task<R1> Match<R1>(Func<R, R1> leftFunc, Func<Error, R1> rightFunc)
    //     {
    //         return await _source.Match(Core, e => Task.FromResult(rightFunc(e)));
    //
    //         async Task<R1> Core(T value)
    //         {
    //             return await _selector(value).Match(leftFunc, rightFunc);
    //         }
    //     }
    //
    //     public async Task<R1> Match<R1>(Func<R, Task<R1>> leftFunc, Func<Error, Task<R1>> rightFunc)
    //     {
    //         return await _source.Match(Core, rightFunc);
    //         
    //         async Task<R1> Core(T value)
    //         {
    //             return await _selector(value).Match(leftFunc, rightFunc);
    //         }
    //     }
    //
    //     public Task<Result<R>> AsTask()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public ResultAwaiter<R> GetAwaiter()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     //TODO: проверить работу функции
    //     public async Task Match(Action<R> leftFunc, Action<Error> rightFunc)
    //     {
    //         await _source.Match(x =>
    //         { 
    //             _selector(x).Match(leftFunc, rightFunc);
    //         }, rightFunc);
    //     }
    // }
}