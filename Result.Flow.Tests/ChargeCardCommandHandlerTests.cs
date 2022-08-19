using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Result.Flow.API;
using Result.Flow.BusinessRules;
using Result.Flow.Interfaces;
using Result.Flow.Persistence;
using Result.Flow.Result;
using Result.Flow.Tests.Common;
using Xunit;

namespace Result.Flow.Tests;

public class ChargeCardCommandHandlerTests : IClassFixture<TestFixture>
{
    private readonly Guid _transactionId = Guid.NewGuid();
    
    private readonly TestFixture _testFixture;
    private readonly IBillingService _billingService;

    private readonly IUserBusinessRule _userBusinessRule;
    private readonly IBusinessRuleFactory _businessRuleFactory;

    public ChargeCardCommandHandlerTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
        _billingService = Substitute.For<IBillingService>();

        _userBusinessRule = Substitute.For<IUserBusinessRule>();
        _businessRuleFactory = _testFixture.CreateRuleFactory(_userBusinessRule);
    }
    
    [Fact]
    public async Task UserNotFound_ReturnsFail()
    {
        var command = new ChargeCardCommand { UserId = long.MaxValue, Amount = 20 };
        var handler = new ChargeCardCommandHandler(
            _testFixture.DbDbContext,
            _billingService,
            _businessRuleFactory);

        var result = await handler.Handle(command, CancellationToken.None);
        
        result.ShouldBeFail();
    }
    
    [Fact]
    public async Task UserCanNotPay_ReturnsFail()
    {
        _userBusinessRule.CardIsValid()
            .Returns(new Error("User Can not pay"));
            
        var command = new ChargeCardCommand { UserId = 1, Amount = 20 };
        var handler = new ChargeCardCommandHandler(
            _testFixture.DbDbContext,
            _billingService,
            _businessRuleFactory);

        var result = await handler.Handle(command, CancellationToken.None);
        result.ShouldBeFail("User Can not pay");
    }
    
    [Fact]
    public async Task BillingException_ReturnsFail()
    {
        _userBusinessRule.CardIsValid()
            .Returns(Result.Result.Ok);

        _billingService.ChargeCard(Arg.Any<CreditCard>(), Arg.Any<decimal>())
            .Returns(new Error("Card payment could not be made"));
        
        _billingService.ChargeCardAsync(Arg.Any<CreditCard>(), Arg.Any<decimal>())
            .Returns(AsyncResult.AsyncResult.Fail<Guid>(new Error("Card payment could not be made")));
        
        var command = new ChargeCardCommand { UserId = 1, Amount = 20 };
        var handler = new ChargeCardCommandHandler(
            _testFixture.DbDbContext,
            _billingService,
            _businessRuleFactory);

        var result = await handler.Handle(command, CancellationToken.None);
        result.ShouldBeFail("Card payment could not be made");
    }
    
    [Fact]
    public async Task CorrectPath_ReturnsOk()
    {
        _userBusinessRule.CardIsValid()
            .Returns(Result.Result.Ok);

        _billingService.ChargeCard(Arg.Any<CreditCard>(), Arg.Any<decimal>())
            .Returns(_transactionId);
        
        _billingService.ChargeCardAsync(Arg.Any<CreditCard>(), Arg.Any<decimal>())
            .Returns(AsyncResult.AsyncResult.Ok(_transactionId));
        
        var command = new ChargeCardCommand { UserId = 1, Amount = 20 };
        var handler = new ChargeCardCommandHandler(
            _testFixture.DbDbContext,
            _billingService,
            _businessRuleFactory);

        var result = await handler.Handle(command, CancellationToken.None);
        result.ShouldBeOk(_transactionId);
    }
}