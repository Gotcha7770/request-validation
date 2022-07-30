using System.Linq;
using Xunit;

namespace Request.Validation.Tests;

public class EnumerableTests
{
    [Fact]
    public void Async_Linq_Expressions()
    {
        var result1 = from x in AsyncEnumerable.Range(0, 10)
                      from y in AsyncEnumerable.Range(0, 10)
                      from z in AsyncEnumerable.Range(0, 10)
                      select x + y + z;
        
        var result2 = AsyncEnumerable.Range(0, 10)
            .SelectMany(x => AsyncEnumerable.Range(0, 10), (x, y) => new { x, y })
            .SelectMany(t => AsyncEnumerable.Range(0, 10), (t, z) => t.x + t.y + z);
        
        var result3 = Enumerable.Range(0, 10)
            .SelectMany(_ => Enumerable.Range(0, 10), (x, y) => new { x, y })
            .ToAsyncEnumerable()
            .SelectMany(_ => AsyncEnumerable.Range(0, 10), (t, z) => t.x + t.y + z);
    }
}