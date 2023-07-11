using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

public class RaceEvent : CommonEventBase
{
    public RaceTrack Track { get; set; }
    public RaceParticipant[] Participants { get; set; }
    public RaceSnapshotOnSector[] Sectors { get; set; }

    public void AddParticipant(Guid userId)
    {
        if (Participants.FirstOrDefault(x => x.UserId == userId) is not null)
        {
            return;
        }

        var freeDrivers = Participants.Where(x => x.UserId is null).ToArray();
        if (freeDrivers.Length == 0)
        {
            throw new NoFreeDriversInRaceException(Id);
        }
        
        freeDrivers.SelectRandomItem().UserId = userId;
    }

    public override CommonEventType Type => CommonEventType.Race;

    public static RaceEvent Create()
    {
        return new RaceEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.Now,
        };
    }
}