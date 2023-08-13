namespace AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

public class RaceEventDto : CommonEventBaseDto
{
    public RaceTrackDto Track { get; set; }
    public int TotalLaps { get; set; }
    public RaceParticipantDto[] Participants { get; set; }
    public RaceSnapshotOnSectorDto[] Sectors { get; set; }

    public override CommonEventTypeDto Type => CommonEventTypeDto.Race;
}