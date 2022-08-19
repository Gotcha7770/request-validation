using Result.Flow.Persistence;
using Result.Flow.Result;

namespace Result.Flow.BusinessRules;

public interface IUserBusinessRule
{
    Result<CreditCard> HasCard();

    Result<Unit> CardIsNotExpired();

    Result<Unit> CardIsValid();
}