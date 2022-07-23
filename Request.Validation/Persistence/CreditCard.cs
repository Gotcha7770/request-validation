namespace Request.Validation.Persistence;

public class CreditCard
{
    public string Number { get; set; }
    public DateOnly Expiry { get; set; }
    public int Cvv { get; set; }
}