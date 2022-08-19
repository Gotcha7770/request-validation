namespace Result.Flow.Result;

public class Result<T> : Either<T, Error>
{
    public Result(T value) : base(value) { }

    public Result(Error error) : base(error) { }
    
    public static implicit operator Result<T>(T value) => new Result<T>(value);

    public static implicit operator Result<T>(Error error) => new Result<T>(error);
}

public class Result : Result<Unit>
{
    public Result() : base(Unit.Value) { }

    public Result(Error error) : base(error) { }

    public static Result Ok { get; } = new Result();
    
    public static implicit operator Result(Error error) => new Result(error);
}