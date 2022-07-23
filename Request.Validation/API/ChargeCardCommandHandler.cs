using MediatR;
using Microsoft.EntityFrameworkCore;
using Request.Validation.BusinessRules;
using Request.Validation.Interfaces;
using Request.Validation.Persistence;

namespace Request.Validation.API;

public class ChargeCardCommandHandler : IRequestHandler<ChargeCardCommand, Result<Guid>>
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IBillingService _billingService;
    private readonly IBusinessRuleFactory _businessRuleFactory;

    public ChargeCardCommandHandler(
        ApplicationDbContext applicationDbContext,
        IBillingService billingService,
        IBusinessRuleFactory businessRuleFactory)
    {
        _applicationDbContext = applicationDbContext;
        _billingService = billingService;
        _businessRuleFactory = businessRuleFactory;
    }
    
    public async Task<Result<Guid>> Handle(ChargeCardCommand request, CancellationToken cancellationToken)
    {
        var lookup = await _applicationDbContext.Users
            .AsNoTracking()
            .Include(x => x.CreditCard)
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
            .AsResult();

        return from user in lookup
               from canPay in _businessRuleFactory.For(user).CardIsValid()
               from charge in _billingService.ChargeCard(user.CreditCard, request.Amount)
               select charge;
    }
}