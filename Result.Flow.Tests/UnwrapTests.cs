using System.Threading.Tasks;
using Functional.Option;
using Result.Flow.Result;
using Xunit;

namespace Result.Flow.Tests;

public class UnwrapTests
{
    private Task<Result<Option<T>>> Do<T>()
    {
        // in ideal world we have a function: () => T
        
        // in real, it could returns "none", let's wrap it in a type: () => Option<T>
        var option = Option.Some(default(T));
        
        // also it could returns exception, second type: () => Result<Option<T>>
        var result = new Result<Option<T>>(option);
        
        // finally function could be async, so yet another time: () => Task<Result<Option<T>>>
        return Task.FromResult(result);
    }
    
    [Fact]
    public async Task Lest_Unwrap_This()
    {
        var task = Do<int>();
        var result = await Do<int>();
        var option = (await Do<int>()).Match(o => o , error => Option.Some(1));
        var value = (await Do<int>()).Match(o => o.Match( None: () => 0, Some: x => x * x) , error => -1);
    }
}