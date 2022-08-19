using System;
using System.Threading.Tasks;
using Result.Flow;
using Result.Flow.AsyncResult;
using Result.Flow.Result;
using Xunit;

namespace Result.Flow.Tests;

public class ResultTests
{
    [Fact]
    public void Result_LINQ_Test()
    {
        var resultLINQ = from x in new Result<int>(1)
                         from y in new Result<int>(2)
                         from z in new Result<int>(3)
                         select x + y + z;

        var output = resultLINQ.Match(x => x, _ => 0);
        
        var resultFluent = new Result<int>(1)
            .SelectMany(x => new Result<int>(2), (x, y) => new { x, y })
            .SelectMany(t => new Result<int>(3), (t, z) => t.x + t.y + z);
    }
    
    [Fact]
    public async Task AsyncResult_LINQ_Test()
    {
        var resultLINQ = from x in AsyncResult.AsyncResult.Ok(1)
                         from y in AsyncResult.AsyncResult.Ok(2)
                         from z in AsyncResult.AsyncResult.Ok(3)
                         select x + y + z;

        var output = await resultLINQ.Match(x => x, _ => 0);
        
        var resultFluent = AsyncResult.AsyncResult.Ok(1)
            .SelectMany(x => AsyncResult.AsyncResult.Ok(2), (x, y) => new { x, y })
            .SelectMany(t => AsyncResult.AsyncResult.Ok(3), (t, z) => t.x + t.y + z);
    }
}