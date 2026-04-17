using AllTheBeans.Application.Beans.Dtos;
using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;
using FluentValidation;
using MediatR;

namespace AllTheBeans.Application.Beans.Commands;

public sealed record CreateBeanCommand(
    string Name,
    string Description,
    string Country,
    string Colour,
    decimal Cost,
    string ImageUrl) : IRequest<BeanDto>;

public sealed class CreateBeanValidator : AbstractValidator<CreateBeanCommand>
{
    public CreateBeanValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.Colour).MaximumLength(50);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImageUrl).MaximumLength(1000);
    }
}

public sealed class CreateBeanHandler(IBeanRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateBeanCommand, BeanDto>
{
    public async Task<BeanDto> Handle(CreateBeanCommand request, CancellationToken ct)
    {
        var bean = new Bean(Guid.NewGuid(), request.Name, request.Description,
                            request.Country, request.Colour, request.Cost, request.ImageUrl);
        await repo.AddAsync(bean, ct);
        await uow.SaveChangesAsync(ct);
        return BeanDto.From(bean);
    }
}