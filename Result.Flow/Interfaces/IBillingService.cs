using Result.Flow.Persistence;
using Result.Flow.Result;
using Result.Flow.AsyncResult;

namespace Result.Flow.Interfaces;

public interface IBillingService
{
    Result<Guid> ChargeCard(CreditCard userCreditCard, decimal requestAmount);
    
    AsyncResult<Guid> ChargeCardAsync(CreditCard userCreditCard, decimal requestAmount);
}