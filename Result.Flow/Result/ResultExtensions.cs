#nullable enable
using Result.Flow.AsyncResult;
using Result.Flow.Interfaces;

namespace Result.Flow.Result;

public static class ResultExtensions
{
    internal static class ErrorFunc<T>
    {
        internal static readonly Func<Error, Result<T>> Value = Error;

        private static Result<T> Error(Error error) => error;
    }
    
    public static Result<R> Select<T, R>(this Result<T> source, Func<T, R> selector)
    {
        return source.Match(x => selector(x), ErrorFunc<R>.Value);
    }
    
    public static Result<R> SelectMany<T, R>(this Result<T> source, Func<T, Result<R>> selector)
    {
        return source.Match(selector, ErrorFunc<R>.Value);
    }
    
    public static IAsyncResult<R> SelectMany<T, R>(this Result<T> source, Func<T, IAsyncResult<R>> selector)
    {
        return source.Match(selector, AsyncResult.AsyncResult.Fail<R>);
    }
    
    public static Result<R2> SelectMany<T, R1, R2>(
        this Result<T> source,
        Func<T, Result<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        return source.Match(
            x => selector(x).Select(y => resultSelector(x, y)),
            ErrorFunc<R2>.Value);
    }

    public static IAsyncResult<R2> SelectMany<T, R1, R2>(
        this Result<T> source,
        Func<T, IAsyncResult<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        return source.Match(
            x => selector(x).Select(y => resultSelector(x, y)),
            AsyncResult.AsyncResult.Fail<R2>);
    }

    //public static Result<T> AsResult<T>(this T item) => item is null ? new Error("Item not found") : item;

    public static Result<T> AsResult<T>(this T? item)
    {
        return item is null ? new Error("Item not found") : item;
    }

    //public static async Task<Result<T>> AsResult<T>(this Task<T> task) => (Result<T>)await task ?? new Error("Item not found");

    public static async Task<Result<T>> AsResult<T>(this Task<T?> task)
    {
        return await task is { } item ? item : new Error("Item not found");
    }
}