namespace AntiClown.F1Predictions.Dto;

public record ChampionshipDetailsDto(
    int Season,
    bool Configured,
    string? RulesVersion,
    ChampionshipStageDto? CurrentStage,
    bool StageStarted,
    bool PredictionsOpen,
    string[] PreSeasonRoster,
    string[] MidSeasonRoster,
    string[] DriverStandings,
    ChampionshipStandingsUpdateDto? StandingsUpdate,
    ChampionshipPredictionDto? PreSeasonPrediction,
    ChampionshipPredictionDto? MidSeasonPrediction,
    ChampionshipParticipantPointsDto[] Rows,
    TeamDto[] Teams,
    string[] CurrentDrivers);
