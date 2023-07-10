using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

public class RaceEvent : CommonEventBase
{
    public RaceTrack Track { get; set; }
    public RaceParticipant[] Participants { get; set; }
    public RaceSnapshotOnSector[] Sectors { get; set; }

    public override CommonEventType Type => CommonEventType.Race;

    public static RaceEvent Create()
    {
        return new RaceEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.UtcNow,
        };
    }
}