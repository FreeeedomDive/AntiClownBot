namespace AntiClown.F1Predictions.Dto;

public record ChampionshipPredictionDto(
    ChampionshipStageDto Stage,
    string[] DriverStandings,
    ChampionshipPredictionSourceDto PredictionSource,
    int[] Points);
