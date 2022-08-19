using Result.Flow.Persistence;
using Result.Flow.Result;

namespace Result.Flow.Interfaces;

public interface IBillingService
{
    Result<Guid> ChargeCard(CreditCard userCreditCard, decimal requestAmount);
    
    IAsyncResult<Guid> ChargeCardAsync(CreditCard userCreditCard, decimal requestAmount);
}