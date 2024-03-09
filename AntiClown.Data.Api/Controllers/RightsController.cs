using AntiClown.Data.Api.Core.Rights.Domain;
using AntiClown.Data.Api.Core.Rights.Services;
using AntiClown.Data.Api.Dto.Rights;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Data.Api.Controllers;

[Route("dataApi/rights/{userId:guid}")]
public class RightsController : Controller
{
    public RightsController(
        IRightsService rightsService,
        IMapper mapper
    )
    {
        this.rightsService = rightsService;
        this.mapper = mapper;
    }

    [HttpGet("")]
    public async Task<ActionResult<RightsDto[]>> FindAllUserRights([FromRoute] Guid userId)
    {
        var result = rightsService.FindAllUserRights(userId);
        return mapper.Map<RightsDto[]>(result);
    }

    [HttpPost("grant")]
    public async Task<ActionResult> GrantAsync([FromRoute] Guid userId, [FromQuery] RightsDto right)
    {
        await rightsService.GrantAsync(userId, mapper.Map<Rights>(right));
        return NoContent();
    }

    [HttpDelete("revoke")]
    public async Task<ActionResult> RevokeAsync(Guid userId, RightsDto right)
    {
        await rightsService.RevokeAsync(userId, mapper.Map<Rights>(right));
    }

    private readonly IRightsService rightsService;
    private readonly IMapper mapper;
}