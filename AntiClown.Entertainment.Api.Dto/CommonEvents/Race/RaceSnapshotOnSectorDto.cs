namespace AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

public class RaceSnapshotOnSectorDto
{
    public int CurrentLap { get; set; }
    public int SectorIndex { get; set; }
    public RaceSectorTypeDto SectorType { get; set; }
    public RaceSnapshotForDriverOnSectorDto[] DriversOnSector { get; set; }
    public int? FastestLap { get; set; }
}