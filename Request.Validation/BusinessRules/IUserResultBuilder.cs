using Request.Validation.Persistence;

namespace Request.Validation.BusinessRules;

public interface IUserResultBuilder
{
    Result<CreditCard> HasCard();

    Result CardIsValid();
}