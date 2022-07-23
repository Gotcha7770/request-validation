using Request.Validation.Persistence;

namespace Request.Validation.BusinessRules;

public class UserResultBuilder : IUserResultBuilder
{
    private readonly User _user;

    public UserResultBuilder(User user)
    {
        _user = user;
    }
    
    public Result<CreditCard> HasCard()
    {
        return _user.CreditCard is null 
            ? new Error("User has not card") 
            : _user.CreditCard;
    }

    public Result CardIsValid()
    {
        return HasCard().Match(
            x => x.Expiry > DateOnly.FromDateTime(DateTime.Now.Date) 
                ? Result.Ok 
                : new Error("Card expired"), 
            error => error);
    }
}