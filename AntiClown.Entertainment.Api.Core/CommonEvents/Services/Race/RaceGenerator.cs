using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.Common;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;
using AntiClown.Tools.Utility.Extensions;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;

public class RaceGenerator : IRaceGenerator
{
    public RaceGenerator(
        IRaceDriversRepository raceDriversRepository,
        IRaceTracksRepository raceTracksRepository
    )
    {
        this.raceDriversRepository = raceDriversRepository;
        this.raceTracksRepository = raceTracksRepository;
    }

    public async Task<RaceEvent> GenerateAsync()
    {
        var race = RaceEvent.Create();
        var track = (await raceTracksRepository.ReadAllAsync()).SelectRandomItem();
        race.Track = track;
        race.TotalLaps = Constants.RaceLaps;
        var driversModels = await raceDriversRepository.ReadAllAsync();
        var participantsShuffledForStartingGrid = driversModels.Select(
            x => new RaceParticipant
            {
                UserId = null,
                Driver = x,
            }
        ).Shuffle().ToArray();
        race.Participants = participantsShuffledForStartingGrid;
        var startingGrid = GenerateStartingGrid(participantsShuffledForStartingGrid);
        var raceSectors = new List<RaceSnapshotOnSector> { startingGrid };
        for (var lap = 1; lap <= race.TotalLaps; lap++)
        {
            var lapSectors = GenerateSectorsForLap(raceSectors.Last(), track, lap, participantsShuffledForStartingGrid);
            raceSectors.AddRange(lapSectors);
        }

        race.Sectors = raceSectors.ToArray();

        return race;
    }

    private static RaceSnapshotOnSector GenerateStartingGrid(IEnumerable<RaceParticipant> participants)
    {
        return new RaceSnapshotOnSector
        {
            FastestLap = null,
            SectorIndex = 0,
            DriversOnSector = participants.Select(
                (participant, position) => new RaceSnapshotForDriverOnSector
                {
                    DriverName = participant.Driver.DriverName,
                    SectorTime = position * Constants.RaceGridPositionPenalty,
                    TotalTime = position * Constants.RaceGridPositionPenalty,
                }
            ).ToArray(),
        };
    }

    private static RaceSnapshotOnSector[] GenerateSectorsForLap(
        RaceSnapshotOnSector lastSector,
        RaceTrack track,
        int lap,
        RaceParticipant[] participants
    )
    {
        var totalSectorsInOneLap = track.CorneringDifficulty + track.BreakingDifficulty + track.AccelerationDifficulty;
        var result = Enumerable.Range(0, totalSectorsInOneLap).Select(
            _ => new RaceSnapshotOnSector
            {
                CurrentLap = lap,
                SectorIndex = (lap - 1) * totalSectorsInOneLap + 1,
                FastestLap = lastSector.FastestLap,
            }
        ).ToArray();

        for (var i = 0; i < track.CorneringDifficulty; i++)
        {
            result[i].SectorType = RaceSectorType.Cornering;
        }

        for (var i = 0; i < track.BreakingDifficulty; i++)
        {
            result[track.CorneringDifficulty + i].SectorType = RaceSectorType.Breaking;
        }

        for (var i = 0; i < track.AccelerationDifficulty; i++)
        {
            result[track.CorneringDifficulty + track.BreakingDifficulty + i].SectorType = RaceSectorType.Acceleration;
        }

        var perfectSectorTime = track.IdealTime / totalSectorsInOneLap;
        for (var i = 0; i < totalSectorsInOneLap; i++)
        {
            var isFirstSector = i == 0;
            var previousSector = isFirstSector ? lastSector : result[i - 1];
            result[i].FastestLap = previousSector.FastestLap;
            result[i].DriversOnSector = participants.Select(
                driver => GenerateDriverSector(
                    driver,
                    result[i].SectorType,
                    previousSector.DriversOnSector.First(d => d.DriverName == driver.Driver.DriverName),
                    perfectSectorTime,
                    isFirstSector
                )
            ).ToArray();
        }

        var currentLapLastSector = result.Last();
        var currentLapFastestLap = currentLapLastSector.DriversOnSector
                                                       .MinBy(driver => driver.CurrentLapTime)!
                                                       .CurrentLapTime;

        currentLapLastSector.FastestLap = lastSector.FastestLap.HasValue
            ? Math.Min(lastSector.FastestLap.Value, currentLapFastestLap)
            : currentLapFastestLap;
        return result;
    }

    private static RaceSnapshotForDriverOnSector GenerateDriverSector(
        RaceParticipant driver,
        RaceSectorType sectorType,
        RaceSnapshotForDriverOnSector previousDriverSector,
        int perfectSectorTime,
        bool isFirstSectorOfLap
    )
    {
        // почему такие коэффициенты? хороший вопрос...
        var maxCorneringSectorTime = (int)(perfectSectorTime / Math.Pow(driver.Driver.CorneringSkill, 2d / 5));
        var maxAccelerationSectorTime = (int)(perfectSectorTime / Math.Pow(driver.Driver.AccelerationSkill, 2d / 5));
        var maxBreakingSectorTime = (int)(perfectSectorTime / Math.Pow(driver.Driver.BreakingSkill, 2d / 5));
        var sectorTime = sectorType switch
        {
            RaceSectorType.Cornering => Randomizer.GetRandomNumberBetween(perfectSectorTime, maxCorneringSectorTime),
            RaceSectorType.Acceleration => Randomizer.GetRandomNumberBetween(
                perfectSectorTime,
                maxAccelerationSectorTime
            ),
            RaceSectorType.Breaking => Randomizer.GetRandomNumberBetween(perfectSectorTime, maxBreakingSectorTime),
            _ => perfectSectorTime,
        };
        return new RaceSnapshotForDriverOnSector
        {
            DriverName = driver.Driver.DriverName,
            SectorTime = sectorTime,
            CurrentLapTime = (isFirstSectorOfLap ? 0 : previousDriverSector.CurrentLapTime) + sectorTime,
            TotalTime = previousDriverSector.TotalTime + sectorTime,
        };
    }

    private readonly IRaceDriversRepository raceDriversRepository;
    private readonly IRaceTracksRepository raceTracksRepository;
}