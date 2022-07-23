using NSubstitute;
using Request.Validation.BusinessRules;
using Request.Validation.Persistence;

namespace Request.Validation.Tests.Common;

public class TestFixture
{
    public ApplicationDbContext DbDbContext { get; init; }

    public TestFixture()
    {
        DbDbContext = ApplicationDbContextFactory.Create();
    }

    public IBusinessRuleFactory CreateRuleFactory(IUserResultBuilder userResultBuilder)
    {
        var ruleFactory = Substitute.For<IBusinessRuleFactory>();
        ruleFactory.For(Arg.Any<User>()).Returns(userResultBuilder);

        return ruleFactory;
    }
}