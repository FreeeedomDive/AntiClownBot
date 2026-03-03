namespace AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

public class RaceSnapshotForDriverOnSectorDto
{
    public string DriverName { get; set; }
    public int SectorTime { get; set; }
    public int CurrentLapTime { get; set; }
    public int TotalTime { get; set; }
}