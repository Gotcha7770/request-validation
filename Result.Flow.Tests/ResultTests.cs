using System;
using System.Threading.Tasks;
using FluentAssertions;
using Result.Flow.AsyncResult;
using Result.Flow.Result;
using Result.Flow.Tests.Common;
using Xunit;

namespace Result.Flow.Tests;

public class ResultTests
{
    [Fact]
    public void DefaultResultShouldBeFail()
    {
        Result<Unit> result = default;

        result.Should().BeFail();
    }

    [Fact]
    public void MatchOk()
    {
        Result<int> result = 42;

        result.Match(x => x.ToString(), error => "error")
            .Should().Be("42");
    }

    [Fact]
    public void MatchFail()
    {
        Result<int> result = new Error("error");

        result.Match(x => x, error => -1)
            .Should().Be(-1);
    }

    [Fact]
    public void Result_LINQ_Test()
    {
        var resultLINQ = from x in new Result<int>(1)
            from y in new Result<int>(2)
            from z in new Result<int>(3)
            select x + y + z;

        resultLINQ.Should().BeOk(6);

        var resultFluent = new Result<int>(1)
            .SelectMany(x => new Result<int>(2), (x, y) => new { x, y })
            .SelectMany(t => new Result<int>(3), (t, z) => t.x + t.y + z);

        resultLINQ.Should().BeOk(6);
    }

    // [Fact]
    // public async Task AsyncResult_LINQ_Test()
    // {
    //     var resultLINQ = from x in AsyncResult<int>.Ok(1)
    //                      from y in AsyncResult<int>.Ok(2)
    //                      from z in AsyncResult<int>.Ok(3)
    //                      select x + y + z;
    //
    //     var output = await resultLINQ.Match(x => x, _ => 0);
    //     
    //     var resultFluent = AsyncResult<int>.Ok(1)
    //         .SelectMany(x => AsyncResult<int>.Ok(2), (x, y) => new { x, y })
    //         .SelectMany(t => AsyncResult<int>.Ok(3), (t, z) => t.x + t.y + z);
    // }

    [Fact]
    public void MeaningOfAsyncResult()
    {
        var resultOfTask = 
            from x in GetValueOfTask()
            from y in GetValueOfTask()
            select x + y;
        
        var result =
            from x in GetValue()
            from y in GetValue()
            select x + y;
    }

    private Task<Result<int>> GetValueOfTask()
    {
        return Task.FromResult(new Result<int>(1));
    }

    private AsyncResult<int> GetValue()
    {
        return AsyncResult<int>.Ok(1);
    }
}

public static class AdHocExtensions
{
    public static async Task<Result<R>> Select<T, R>(this Task<Result<T>> source, Func<T, R> selector)
    {
        var result = await source;
        return result.Select(selector);
    }

    public static async Task<Result<R>> SelectMany<T, R>(this Task<Result<T>> source, Func<T, Task<Result<R>>> selector)
    {
        var result = await source;
        return await result.Match(selector, e => Task.FromResult(Result<R>.Fail(e)));
    }
    
    public static async Task<Result<R2>> SelectMany<T, R1, R2>(
        this Task<Result<T>> source, 
        Func<T, Task<Result<R1>>> selector,
        Func<T, R1, R2> resultSelector)
    {
        var result = await source;
        return await result.Match(
            x => selector(x).Select(y => resultSelector(x, y)),
            e => Task.FromResult(Result<R2>.Fail(e)));
    }
}