using Result.Flow.Interfaces;
using Result.Flow.Result;

namespace Result.Flow.AsyncResult;

public static partial class AsyncResultExtensions
{
    // public static async AsyncResult<T> AsAsyncResult<T>(this Task<T?> task)
    // {
    //     return await task is { } item ? item : new Error("Item not found");
    // }

    public static IAsyncResult<R> Select<T, R>(this IAsyncResult<T> source, Func<T, R> selector)
    {
        var task = source.Match(x => selector(x), ResultExtensions.ErrorFunc<R>.Value);
        return new AsyncResult<R>(task);
    }
    
    public static IAsyncResult<R> SelectMany<T, R>(this IAsyncResult<T> source, Func<T, IAsyncResult<R>> selector)
    {
        return new SelectManyAsyncResult<T, R>(source, selector);

        //return Core(item, selector);
        
        // lack of language support(((
        // static async AsyncResult<R> Core(AsyncResult<T> source, Func<T, AsyncResult<R>> selector)
        // {
        //     var result = await source.Match(selector, error => Task.FromResult(new Result<R>(error)));
        // }
    }

    public static IAsyncResult<R2> SelectMany<T, R1, R2>(
        this IAsyncResult<T> source,
        Func<T, AsyncResult<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        var task = source.Match( 
            x => selector(x).Match(y => resultSelector(x, y), ResultExtensions.ErrorFunc<R2>.Value), 
            Error);

        return new AsyncResult<R2>(task);

        Task<Result<R2>> Error(Error error) => Task.FromResult(new Result<R2>(error));
    }

    class SelectManyAsyncResult<T, R> : IAsyncResult<R>
    {
        private readonly IAsyncResult<T> _source;
        private readonly Func<T, IAsyncResult<R>> _selector;

        public SelectManyAsyncResult(IAsyncResult<T> source, Func<T, IAsyncResult<R>> selector)
        {
            _source = source;
            _selector = selector;
        }

        public Task<bool> IsSuccess() => _source.IsSuccess();
        
        public async Task<R1> Match<R1>(Func<R, R1> leftFunc, Func<Error, R1> rightFunc)
        {
            return await _source.Match(Core, e => Task.FromResult(rightFunc(e)));

            async Task<R1> Core(T value)
            {
                return await _selector(value).Match(leftFunc, rightFunc);
            }
        }

        public async Task<R1> Match<R1>(Func<R, Task<R1>> leftFunc, Func<Error, Task<R1>> rightFunc)
        {
            return await _source.Match(Core, rightFunc);
            
            async Task<R1> Core(T value)
            {
                return await _selector(value).Match(leftFunc, rightFunc);
            }
        }

        public Task<Result<R>> AsTask()
        {
            throw new NotImplementedException();
        }

        public ResultAwaiter<R> GetAwaiter()
        {
            throw new NotImplementedException();
        }

        //TODO: проверить работу функции
        public async Task Match(Action<R> leftFunc, Action<Error> rightFunc)
        {
            await _source.Match(x =>
            { 
                _selector(x).Match(leftFunc, rightFunc);
            }, rightFunc);
        }
    }
}