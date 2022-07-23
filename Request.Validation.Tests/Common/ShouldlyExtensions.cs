using Shouldly;

namespace Request.Validation.Tests.Common;

public static class ShouldlyExtensions
{
    public static void ShouldBeFail<T>(this Result<T> actual, string message = default, string customMessage = default)
    {
        actual.Match(
            x =>
            {
                Result<T> error = new Error(message);
                var shouldlyMessage = new ExpectedActualShouldlyMessage(error, x, customMessage);
                throw new ShouldAssertException(shouldlyMessage.ToString());
            },
            error =>
            {
                if (message is not null)
                {
                    error.Message.ShouldBe(message);
                }
            });
    }
    
    public static void ShouldBeOk<T>(this Result<T> actual, T expected, string customMessage = default)
    {
        actual.Match(
            x => x.ShouldBe(expected),
            error =>
            {
                var message = new ExpectedActualShouldlyMessage(expected, error, customMessage);
                throw new ShouldAssertException(message.ToString());
            });
    }
    
    public static void ShouldBeOk(this Result actual, string customMessage = default)
    {
        actual.ShouldBeOk(Unit.Value, customMessage);
    }
}