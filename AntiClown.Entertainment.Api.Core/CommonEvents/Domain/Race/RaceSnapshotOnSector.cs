namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;

public class RaceSnapshotOnSector
{
    public int CurrentLap { get; set; }
    public int SectorIndex { get; set; }
    public RaceSectorType SectorType { get; set; }
    public RaceSnapshotForDriverOnSector[] DriversOnSector { get; set; }
    public int? FastestLap { get; set; }
}