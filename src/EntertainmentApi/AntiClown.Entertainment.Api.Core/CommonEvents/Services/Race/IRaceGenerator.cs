using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;

public interface IRaceGenerator
{
    Task<RaceEvent> GenerateAsync();
}