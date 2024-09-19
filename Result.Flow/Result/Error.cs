namespace Result.Flow.Result;

public record Error(string Message)
{
    internal static readonly Error Default = new(string.Empty);
    public bool IsDefault => Equals(Default);
}