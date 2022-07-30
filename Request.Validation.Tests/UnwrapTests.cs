using System.Threading.Tasks;
using Xunit;

namespace Request.Validation.Tests;

public class UnwrapTests
{
    private Task<Result<T>> Do<T>()
    {
        return Task.FromResult(new Result<T>(default(T)));
    }
    
    [Fact]
    public async Task UnwrapTest()
    {
        var result = Do<int>();
        var result1 = await Do<int>();
        var result2 = (await Do<int>()).Match(x => x * x, _ => -1);
        var result4 = await Do<int>().Select(x => x.Match(r => r * r, _ => -1));
    }
    
}