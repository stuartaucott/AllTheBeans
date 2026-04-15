using AllTheBeans.Application.Beans.Dtos;
using AllTheBeans.Application.Common;
using MediatR;

namespace AllTheBeans.Application.Beans.Queries;

public sealed record GetBeanByIdQuery(Guid Id) : IRequest<BeanDto?>;

public sealed class GetBeanByIdHandler(IBeanRepository repo)
    : IRequestHandler<GetBeanByIdQuery, BeanDto?>
{
    public async Task<BeanDto?> Handle(GetBeanByIdQuery request, CancellationToken ct)
    {
        var bean = await repo.GetAsync(request.Id, ct);
        return bean is null ? null : BeanDto.From(bean);
    }
}
