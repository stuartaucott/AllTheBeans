using AllTheBeans.Application.Beans.Commands;
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
    [Authorize]
    public async Task<IReadOnlyList<BeanDto>> Search(
    string? query, string? country, string? colour,
    decimal? minCost, decimal? maxCost,
    int page = 1, int pageSize = 20,
    CancellationToken ct = default)
    {
        var request = new SearchBeansQuery(query, country, colour, minCost, maxCost, page, pageSize);
        return await mediator.Send(request, ct);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<BeanDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await mediator.Send(new GetBeanByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
    [HttpPost]
    [Authorize(Policy = "Writer")]
    [ProducesResponseType(typeof(BeanDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<BeanDto>> Create([FromBody] CreateBeanCommand cmd, CancellationToken ct)
    {
        var created = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Writer")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBeanCommand body, CancellationToken ct)
    {
        if (id != body.Id) return BadRequest("Route id / body id mismatch.");
        var ok = await mediator.Send(body, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Writer")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await mediator.Send(new DeleteBeanCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }
}
