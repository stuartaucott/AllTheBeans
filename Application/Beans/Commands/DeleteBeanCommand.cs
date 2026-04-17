using AllTheBeans.Application.Common;
using MediatR;

namespace AllTheBeans.Application.Beans.Commands;

public sealed record DeleteBeanCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteBeanHandler(IBeanRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteBeanCommand, bool>
{
    public async Task<bool> Handle(DeleteBeanCommand request, CancellationToken ct)
    {
        var existing = await repo.GetAsync(request.Id, ct);
        if (existing is null) return false;
        await repo.DeleteAsync(request.Id, ct);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}