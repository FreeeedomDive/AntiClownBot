using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

public class RaceParticipant
{
    public RaceDriver Driver { get; set; }
    public Guid? UserId { get; set; }
}