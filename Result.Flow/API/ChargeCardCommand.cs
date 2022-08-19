using MediatR;
using Result.Flow.Result;

namespace Result.Flow.API;

public class ChargeCardCommand : IRequest<Result<Guid>>
{
    public long UserId { get; set; }
    public decimal Amount { get; set; }
}