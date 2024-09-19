#nullable enable
using Result.Flow.AsyncResult;
using Result.Flow.Interfaces;

namespace Result.Flow.Result;

public static partial class ResultExtensions
{
    public static Result<R> Select<T, R>(this Result<T> source, Func<T, R> selector)
    {
        return source.Match(x => selector(x), Result<R>.Fail);
    }
    
    public static Result<R> SelectMany<T, R>(this Result<T> source, Func<T, Result<R>> selector)
    {
        return source.Match(selector, Result<R>.Fail);
    }
    
    public static AsyncResult<R> SelectMany<T, R>(this Result<T> source, Func<T, AsyncResult<R>> selector)
    {
        return source.Match(selector, AsyncResult<R>.Fail);
    }
    
    public static Result<R2> SelectMany<T, R1, R2>(
        this Result<T> source,
        Func<T, Result<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        return source.Match(
            x => selector(x).Select(y => resultSelector(x, y)),
            Result<R2>.Fail);
    }

    public static AsyncResult<R2> SelectMany<T, R1, R2>(
        this Result<T> source,
        Func<T, AsyncResult<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        return source.Match(
            x => selector(x).Select(y => resultSelector(x, y)),
            AsyncResult<R2>.Fail);
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