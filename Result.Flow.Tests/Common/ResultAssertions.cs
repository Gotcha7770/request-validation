using FluentAssertions;
using FluentAssertions.Execution;
using Result.Flow.Result;

namespace Result.Flow.Tests.Common;

public class ResultAssertions<T> : ResultAssertions<T, ResultAssertions<T>>
{
    public ResultAssertions(Result<T> instance) : base(instance) { }
}

public class ResultAssertions<T, TAssertions>
    where TAssertions : ResultAssertions<T, TAssertions>
{
    private readonly Result<T> _instance;

    public ResultAssertions(Result<T> instance)
    {
        _instance = instance;
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> BeOk(T expected, string because = "", params object[] becauseArgs)
    {
        _instance.Match(
            x => Execute.Assertion
                .ForCondition(x.Equals(expected))
                .BecauseOf(because, becauseArgs)
                .FailWith("Result instance should be Ok with value {0} but found {1}", expected, x),
            error => Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .FailWith("Result instance should be Ok, but found fail with message {0}", error.Message));
        
        return new AndConstraint<TAssertions>((TAssertions)this);
    }

    [CustomAssertion]
    public AndConstraint<TAssertions> BeFail(string because = "", params object[] becauseArgs)
    {
        _instance.Match(
            x => Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .FailWith("Result instance should be Fail, but found Ok with value {0}", x),
            error => { });
        
        return new AndConstraint<TAssertions>((TAssertions)this);
    }
}