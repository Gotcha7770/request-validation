namespace Request.Validation;

public class Result<T> : Either<T, Error>
{
    public Result(T left) : base(left)
    {
    }

    public Result(Error right) : base(right)
    {
    }
    
    public static implicit operator Result<T>(T left) => new Result<T>(left);

    public static implicit operator Result<T>(Error right) => new Result<T>(right);
}

public class Result : Result<Unit>
{
    public Result() : base(Unit.Value)
    {
    }

    public Result(Error right) : base(right)
    {
    }

    public static Result Ok { get; } = new Result();
    
    public static implicit operator Result(Error right) => new Result(right);
}