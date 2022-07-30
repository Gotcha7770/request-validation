using Request.Validation.Persistence;

namespace Request.Validation.Interfaces;

public interface IBillingService
{
    Result<Guid> ChargeCard(CreditCard userCreditCard, decimal requestAmount);

    //Task<Result<Guid>> ChargeCard(CreditCard userCreditCard, decimal requestAmount);
    IAsyncResult<Guid> ChargeCardAsync(CreditCard userCreditCard, decimal requestAmount);
}