using NSubstitute;
using Result.Flow.BusinessRules;
using Result.Flow.Persistence;

namespace Result.Flow.Tests.Common;

public class TestFixture
{
    public ApplicationDbContext DbDbContext { get; init; }

    public TestFixture()
    {
        DbDbContext = ApplicationDbContextFactory.Create();
    }

    public IBusinessRuleFactory CreateRuleFactory(IUserBusinessRule userBusinessRule)
    {
        var ruleFactory = Substitute.For<IBusinessRuleFactory>();
        ruleFactory.For(Arg.Any<User>()).Returns(userBusinessRule);

        return ruleFactory;
    }
}