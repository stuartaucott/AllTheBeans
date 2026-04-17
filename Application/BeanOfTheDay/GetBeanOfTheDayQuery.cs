using AllTheBeans.Application.Beans.Dtos;
using MediatR;

namespace AllTheBeans.Application.BeanOfTheDay;

public sealed record GetBeanOfTheDayQuery : IRequest<BeanDto?>;

public sealed class GetBeanOfTheDayHandler(IBeanOfTheDayService service)
    : IRequestHandler<GetBeanOfTheDayQuery, BeanDto?>
{
    public Task<BeanDto?> Handle(GetBeanOfTheDayQuery _, CancellationToken ct)
        => service.GetOrSelectAsync(ct);
}