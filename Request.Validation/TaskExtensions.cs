namespace Request.Validation;

public static class TaskExtensions
{
    public static async Task<R> Select<T, R>(this Task<T> item, Func<T, R> selector)
    {
        return selector(await item);
    }
    
    public static async Task<R> SelectMany<T, R>(this Task<T> item, Func<T, Task<R>> selector)
    {
        return await selector(await item);
    }
    
    public static async Task<R2> SelectMany<T, R1, R2>(
        this Task<T> item,
        Func<T, Task<R1>> selector,
        Func<T, R1, R2> resultSelector)
    {
        return await item.SelectMany(async x => resultSelector(x, await selector(x)));
    }
}