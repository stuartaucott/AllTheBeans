using AllTheBeans.Application.Beans.Dtos;
using AllTheBeans.Application.Beans.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllTheBeans.Controllers;

[ApiController]
[Route("api/beans")]
public sealed class BeansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IReadOnlyList<BeanDto>> Search(
    [FromQuery] SearchBeansQuery query,
    CancellationToken ct) => await mediator.Send(query, ct);

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BeanDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await mediator.Send(new GetBeanByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
