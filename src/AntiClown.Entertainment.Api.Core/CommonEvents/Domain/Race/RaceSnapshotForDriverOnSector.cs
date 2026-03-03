namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

public class RaceSnapshotForDriverOnSector
{
    public string DriverName { get; set; }
    public int SectorTime { get; set; }
    public int CurrentLapTime { get; set; }
    public int TotalTime { get; set; }
}