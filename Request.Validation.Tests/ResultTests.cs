using System;
using System.Threading.Tasks;
using Xunit;

namespace Request.Validation.Tests;

public class ResultTests
{
    [Fact]
    public void Result_LINQ_Test()
    {
        var result1 = from x in new Result<int>(1)
                      from y in new Result<int>(2)
                      from z in new Result<int>(3)
                      select x + y + z;

        var output = result1.Match(x => x, _ => 0);
        
        var result2 = new Result<int>(1)
            .SelectMany(x => new Result<int>(2), (x, y) => new { x, y })
            .SelectMany(t => new Result<int>(3), (t, z) => t.x + t.y + z);
    }
    
    [Fact]
    public async Task AsyncResult_LINQ_Test()
    {
        var result1 = from x in AsyncResult.Ok(1)
                      from y in AsyncResult.Ok(2)
                      from z in AsyncResult.Ok(3)
                      select x + y + z;

        var output = await result1.Match(x => x, _ => 0);
        
        var result2 = AsyncResult.Ok(1)
            .SelectMany(x => AsyncResult.Ok(2), (x, y) => new { x, y })
            .SelectMany(t => AsyncResult.Ok(3), (t, z) => t.x + t.y + z);
    }
}