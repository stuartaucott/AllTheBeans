using AllTheBeans.Application.Common;
using FluentValidation;
using MediatR;

namespace AllTheBeans.Application.Beans.Commands;

public sealed record UpdateBeanCommand(
    Guid Id,
    string Name,
    string Description,
    string Country,
    string Colour,
    decimal Cost,
    string ImageUrl) : IRequest<bool>;

public sealed class UpdateBeanValidator : AbstractValidator<UpdateBeanCommand>
{
    public UpdateBeanValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.Colour).MaximumLength(50);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImageUrl).MaximumLength(1000);
    }
}

public sealed class UpdateBeanHandler(IBeanRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateBeanCommand, bool>
{
    public async Task<bool> Handle(UpdateBeanCommand request, CancellationToken ct)
    {
        var bean = await repo.GetAsync(request.Id, ct);
        if (bean is null) return false;
        bean.Update(request.Name, request.Description, request.Country,
                    request.Colour, request.Cost, request.ImageUrl);
        await repo.UpdateAsync(bean, ct);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}