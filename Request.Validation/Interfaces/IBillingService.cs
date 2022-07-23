using Request.Validation.Persistence;

namespace Request.Validation.Interfaces;

public interface IBillingService
{
    Result<Guid> ChargeCard(CreditCard userCreditCard, decimal requestAmount);
}