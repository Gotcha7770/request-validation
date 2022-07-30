using System.Threading.Tasks;
using Xunit;

namespace Request.Validation.Tests;

public class TaskTests
{
    [Fact]
    public async Task Async_Linq_Expressions()
    {
        var result1 = from x in Task.FromResult(1)
                      from y in Task.FromResult(2)
                      from z in Task.FromResult(3)
                      select x + y + z;

        var output =  await result1;
        
        var result2 = Task.FromResult(1)
            .SelectMany(x => Task.FromResult(2), (x, y) => new { x, y })
            .SelectMany(t => Task.FromResult(3), (t, z) => t.x + t.y + z);
    }

    [Fact]
    public async Task CombineMonadicTypes()
    {
        var result1 = from _x in Task.FromResult(new Result<int>(1))
                      from _y in Task.FromResult(new Result<int>(2))
                      from _z in Task.FromResult(new Result<int>(3))
                      select from x in _x
                             from y in _y
                             from z in _z
                             select x + y + z;

        var output = await result1.Select(x => x.Match(r => r, _ => -1));
    }
}