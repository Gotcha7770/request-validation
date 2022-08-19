using Result.Flow.Persistence;

namespace Result.Flow.BusinessRules;

public interface IBusinessRuleFactory
{
    IUserBusinessRule For(User user);
}