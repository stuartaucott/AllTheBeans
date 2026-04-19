using AllTheBeans.Application.Beans.Dtos;
using AllTheBeans.Application.BeanOfTheDay;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllTheBeans.Controllers;

[ApiController]
[Route("api/bean-of-the-day")]
[Authorize]
public sealed class BeanOfTheDayController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(BeanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeanDto>> Get(CancellationToken ct)
    {
        var dto = await mediator.Send(new GetBeanOfTheDayQuery(), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}