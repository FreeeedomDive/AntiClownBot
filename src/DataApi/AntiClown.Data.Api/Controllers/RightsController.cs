using AntiClown.Data.Api.Core.Rights.Domain;
using AntiClown.Data.Api.Core.Rights.Services;
using AntiClown.Data.Api.Dto.Rights;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Data.Api.Controllers;

[Route("dataApi/rights")]
public class RightsController(
    IRightsService rightsService,
    IMapper mapper
) : Controller
{
    [HttpGet]
    public async Task<ActionResult<Dictionary<RightsDto, Guid[]>>> ReadAll()
    {
        var result = await rightsService.ReadAllAsync();
        return mapper.Map<Dictionary<RightsDto, Guid[]>>(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<RightsDto[]>> FindAllUserRights([FromRoute] Guid userId)
    {
        var result = await rightsService.FindAllUserRightsAsync(userId);
        return mapper.Map<RightsDto[]>(result);
    }

    [HttpPost("{userId:guid}/grant")]
    public async Task<ActionResult> Grant([FromRoute] Guid userId, [FromQuery] RightsDto right)
    {
        await rightsService.GrantAsync(userId, mapper.Map<Rights>(right));
        return NoContent();
    }

    [HttpDelete("{userId:guid}/revoke")]
    public async Task<ActionResult> Revoke([FromRoute] Guid userId, [FromQuery] RightsDto right)
    {
        await rightsService.RevokeAsync(userId, mapper.Map<Rights>(right));
        return NoContent();
    }
}