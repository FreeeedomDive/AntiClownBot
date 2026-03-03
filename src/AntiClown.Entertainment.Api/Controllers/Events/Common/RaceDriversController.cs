using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/race/drivers")]
public class RaceDriversController : Controller
{
    public RaceDriversController(
        IRaceDriversRepository raceDriversRepository,
        IMapper mapper
    )
    {
        this.raceDriversRepository = raceDriversRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<RaceDriverDto[]>> ReadAllAsync()
    {
        var result = await raceDriversRepository.ReadAllAsync();
        return mapper.Map<RaceDriverDto[]>(result);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<RaceDriverDto>> FindAsync([FromRoute] string name)
    {
        var result = await raceDriversRepository.FindAsync(name);
        return mapper.Map<RaceDriverDto>(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] RaceDriverDto raceDriver)
    {
        var driver = mapper.Map<RaceDriver>(raceDriver);
        await raceDriversRepository.CreateAsync(driver);

        return NoContent();
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateAsync([FromBody] RaceDriverDto raceDriver)
    {
        var driver = mapper.Map<RaceDriver>(raceDriver);
        await raceDriversRepository.UpdateAsync(driver);

        return NoContent();
    }

    private readonly IMapper mapper;

    private readonly IRaceDriversRepository raceDriversRepository;
}