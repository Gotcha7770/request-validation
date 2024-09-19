namespace Result.Flow.Result;

public readonly struct Result<T>
{
    private readonly T _value;
    private readonly Error _error = Error.Default;

    public bool IsSuccess { get; }

    public Result(T value)
    {
        _value = value;
        _error = default;
        IsSuccess = true;
    }

    public Result(Error error)
    {
        _value = default;
        _error = error;
        IsSuccess = false;
    }

    public void Match(Action<T> valueHandler, Action<Error> errorHandler)
    {
        if (IsSuccess)
            valueHandler(_value);
        else
            errorHandler(_error);
    }

    public TR Match<TR>(Func<T, TR> valueHandler, Func<Error, TR> errorHandler) => IsSuccess
        ? valueHandler(_value)
        : errorHandler(_error);

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
    
    public static Result<T> Ok(T value) => new(value);
    
    public static Result<T> Fail(Error error) => new(error);
}