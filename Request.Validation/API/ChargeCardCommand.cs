using MediatR;

namespace Request.Validation.API;

public class ChargeCardCommand : IRequest<Result<Guid>>
{
    public long UserId { get; set; }
    public decimal Amount { get; set; }
}