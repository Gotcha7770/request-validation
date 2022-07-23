#nullable enable
namespace Request.Validation;

public static class ResultExtensions
{
    internal static class ErrorFunc<T>
    {
        internal static readonly Func<Error, Result<T>> Value = Error;

        private static Result<T> Error(Error error) => error;
    }
    
    public static Result<R> Select<T, R>(this Result<T> item, Func<T, R> selector)
    {
        return item.Match(x => selector(x), ErrorFunc<R>.Value);
    }
    
    public static Result<R> SelectMany<T, R>(this Result<T> item, Func<T, Result<R>> selector)
    {
        return item.Match(selector, ErrorFunc<R>.Value);
    }
    
    public static Result<R2> SelectMany<T, R1, R2>(
        this Result<T> item,
        Func<T, Result<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        return item.Match(
            x => selector(x).Match(y => resultSelector(x, y), ErrorFunc<R2>.Value),
            ErrorFunc<R2>.Value);
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