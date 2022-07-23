using Request.Validation.Persistence;

namespace Request.Validation.BusinessRules;

public interface IBusinessRuleFactory
{
    IUserResultBuilder For(User user);
}