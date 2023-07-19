using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Entertainment.Api.Core.Parties.Services;
using AntiClown.Entertainment.Api.Dto.Parties;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Parties;

[Route("entertainmentApi/parties")]
public class PartiesController : Controller
{
    public PartiesController(
        IPartiesService partiesService,
        IMapper mapper
    )
    {
        this.partiesService = partiesService;
        this.mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PartyDto>> ReadAsync([FromRoute] Guid id)
    {
        var result = await partiesService.ReadAsync(id);
        return mapper.Map<PartyDto>(result);
    }

    [HttpGet("opened")]
    public async Task<ActionResult<PartyDto[]>> ReadOpenedAsync()
    {
        var result = await partiesService.ReadOpenedAsync();
        return mapper.Map<PartyDto[]>(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CreatePartyDto newParty)
    {
        return await partiesService.CreateAsync(mapper.Map<CreateParty>(newParty));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync([FromBody] PartyDto party)
    {
        await partiesService.UpdateAsync(mapper.Map<Party>(party));
        return NoContent();
    }

    private readonly IMapper mapper;
    private readonly IPartiesService partiesService;
}