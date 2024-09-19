using Result.Flow.Persistence;
using Result.Flow.Result;

namespace Result.Flow.BusinessRules;

public class UserBusinessRule : IUserBusinessRule
{
    private readonly User _user;

    public UserBusinessRule(User user)
    {
        _user = user;
    }
    
    public Result<CreditCard> HasCard()
    {
        return _user.CreditCard is null 
            ? new Error("User has not card") 
            : _user.CreditCard;
    }

    public Result<Unit> CardIsNotExpired()
    {
        return _user.CreditCard.Expiry > DateOnly.FromDateTime(DateTime.Now.Date) 
            ? Unit.Value 
            : new Error("Card expired");
    }

    public Result<Unit> CardIsValid()
    {
        return HasCard().SelectMany(_ => CardIsNotExpired());
    }
}