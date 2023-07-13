using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/race/tracks")]
public class RaceTracksController : Controller
{
    public RaceTracksController(
        IRaceTracksRepository raceTracksRepository,
        IMapper mapper
    )
    {
        this.raceTracksRepository = raceTracksRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<RaceTrackDto[]>> ReadAllAsync()
    {
        var result = await raceTracksRepository.ReadAllAsync();
        return mapper.Map<RaceTrackDto[]>(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] RaceTrackDto raceTrack)
    {
        var track = mapper.Map<RaceTrack>(raceTrack);
        await raceTracksRepository.CreateAsync(track);

        return NoContent();
    }

    private readonly IMapper mapper;

    private readonly IRaceTracksRepository raceTracksRepository;
}