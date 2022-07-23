namespace Request.Validation;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    private static readonly Unit _value = new();
    
    public static ref readonly Unit Value => ref _value;

    public bool Equals(Unit other) => true;
    
    public override bool Equals(object obj) => obj is Unit;

    public static bool operator ==(Unit left, Unit right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Unit left, Unit right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode() => 0;

    public int CompareTo(Unit other) => 0;

    public int CompareTo(object obj) => obj is Unit ? 0 : -1;
}