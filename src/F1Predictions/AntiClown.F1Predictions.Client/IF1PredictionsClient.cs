using AntiClown.F1Predictions.Dto;

namespace AntiClown.F1Predictions.Client;

public interface IF1PredictionsClient
{
    Task<RaceDto[]> GetRacesAsync(Guid participantId, int season, CancellationToken cancellationToken = default);
    Task<UserRaceDto> GetRaceAsync(Guid participantId, Guid raceId, CancellationToken cancellationToken = default);
    Task<PredictionDto> SavePredictionAsync(Guid participantId, Guid raceId, SavePredictionRequestDto prediction, CancellationToken cancellationToken = default);
    Task<SeasonStandingsDto> GetStandingsAsync(int season, CancellationToken cancellationToken = default);
    Task<SeasonStatsDto> GetStatsAsync(Guid participantId, int season, CancellationToken cancellationToken = default);
    Task<TeamDto[]> GetTeamsAsync(Guid participantId, int season, CancellationToken cancellationToken = default);
    Task<ChampionshipDetailsDto> GetChampionshipAsync(Guid participantId, int season, CancellationToken cancellationToken = default);
    Task<ChampionshipPredictionDto> SaveChampionshipPredictionAsync(Guid participantId, int season,
        ChampionshipStageDto stage, SaveChampionshipPredictionRequestDto prediction, CancellationToken cancellationToken = default);
}
