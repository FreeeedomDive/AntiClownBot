namespace AntiClown.F1Predictions.Dto;

public record ChampionshipStandingsUpdateDto(int? LastSyncedRound, DateTimeOffset UpdatedAt);
