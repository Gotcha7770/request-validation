using Result.Flow.Result;
using Shouldly;

namespace Result.Flow.Tests.Common;

public static class ShouldlyExtensions
{
    public static void ShouldBeFail<T>(this Result<T> actual, string message = default, string customMessage = default)
    {
        actual.Match(
            x => ThrowIfValue(message, customMessage, x),
            error => AssertError(message, error));
    }

    private static void ThrowIfValue<T>(string message, string customMessage, T x)
    {
        Result<T> error = new Error(message);
        var shouldlyMessage = new ExpectedActualShouldlyMessage(error, x, customMessage);
        throw new ShouldAssertException(shouldlyMessage.ToString());
    }

    private static void AssertError(string message, Error error)
    {
        if (message is not null)
        {
            error.Message.ShouldBe(message);
        }
    }

    public static void ShouldBeOk<T>(this Result<T> actual, T expected, string customMessage = default)
    {
        actual.Match(
            x => x.ShouldBe(expected),
            error => ThrowIfError(expected, customMessage, error));
    }

    private static void ThrowIfError<T>(T expected, string customMessage, Error error)
    {
        var message = new ExpectedActualShouldlyMessage(expected, error, customMessage);
        throw new ShouldAssertException(message.ToString());
    }

    public static void ShouldBeOk(this Result.Result actual, string customMessage = default)
    {
        actual.ShouldBeOk(Unit.Value, customMessage);
    }
}