using System.Diagnostics.Contracts;
using Result.Flow.Result;

namespace Result.Flow.Tests.Common;

public static class AssertionExtensions
{
    [Pure]
    public static ResultAssertions<T> Should<T>(this Result<T> instance)
    {
        return new ResultAssertions<T>(instance);
    } 
}