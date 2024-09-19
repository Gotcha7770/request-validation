using MediatR;
using Microsoft.EntityFrameworkCore;
using Result.Flow.AsyncResult;
using Result.Flow.BusinessRules;
using Result.Flow.Interfaces;
using Result.Flow.Persistence;
using Result.Flow.Result;

namespace Result.Flow.API;

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

        return await from user in lookup
                     from card in _businessRuleFactory.For(user).HasCard()
                     from isValid in _businessRuleFactory.For(user).CardIsValid()
                     from charge1 in _billingService.ChargeCardAsync(user.CreditCard, request.Amount)
                     from charge2 in _billingService.ChargeCardAsync(user.CreditCard, request.Amount)
                     select charge2;
    }
}