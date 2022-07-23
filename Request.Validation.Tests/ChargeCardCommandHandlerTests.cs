using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Request.Validation.API;
using Request.Validation.BusinessRules;
using Request.Validation.Interfaces;
using Request.Validation.Persistence;
using Request.Validation.Tests.Common;
using Xunit;

namespace Request.Validation.Tests;

public class ChargeCardCommandHandlerTests : IClassFixture<TestFixture>
{
    private readonly Guid _transactionId = Guid.NewGuid();
    
    private readonly TestFixture _testFixture;
    private readonly IBillingService _billingService;

    private readonly IUserResultBuilder _userResultBuilder;
    private readonly IBusinessRuleFactory _businessRuleFactory;

    public ChargeCardCommandHandlerTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
        _billingService = Substitute.For<IBillingService>();

        _userResultBuilder = Substitute.For<IUserResultBuilder>();
        _businessRuleFactory = _testFixture.CreateRuleFactory(_userResultBuilder);
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
        _userResultBuilder.CardIsValid()
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
        _userResultBuilder.CardIsValid()
            .Returns(Result.Ok);

        _billingService.ChargeCard(Arg.Any<CreditCard>(), Arg.Any<decimal>())
            .Returns(new Error("Card payment could not be made"));
        
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
        _userResultBuilder.CardIsValid()
            .Returns(Result.Ok);

        _billingService.ChargeCard(Arg.Any<CreditCard>(), Arg.Any<decimal>())
            .Returns(_transactionId);
        
        var command = new ChargeCardCommand { UserId = 1, Amount = 20 };
        var handler = new ChargeCardCommandHandler(
            _testFixture.DbDbContext,
            _billingService,
            _businessRuleFactory);

        var result = await handler.Handle(command, CancellationToken.None);
        result.ShouldBeOk(_transactionId);
    }
}