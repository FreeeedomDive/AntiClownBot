namespace AntiClown.F1Predictions.Dto;

public record SeasonDriverStatsDto(string Driver, int Score);
public record SeasonParticipantStatsDto(Guid ParticipantId, string ParticipantName, int Score);
public record SeasonSafetyCarStatsDto(SafetyCarsCountDto SafetyCars, int Score);
public record SeasonLeadGapPredictionStatsDto(Guid ParticipantId, string ParticipantName, string RaceName,
    decimal PredictedGap, decimal ActualGap, decimal Difference);
public record SeasonStatsDto(SeasonDriverStatsDto[] TenthPlacePointsRating, SeasonDriverStatsDto[] MostPickedForTenthPlace,
    SeasonDriverStatsDto[] TenthPickedButDnfed, SeasonDriverStatsDto[] DriverOwnTenthPlacePoints,
    SeasonDriverStatsDto[] TenthPlacePredictionEfficiency, SeasonDriverStatsDto[] MostDnfDrivers,
    SeasonDriverStatsDto[] MostPickedForDnf, SeasonParticipantStatsDto[] TenthPlaceDnfAntiRating,
    SeasonLeadGapPredictionStatsDto[] ClosestLeadGapPredictions, SeasonSafetyCarStatsDto[] SafetyCarPickCounts,
    SeasonSafetyCarStatsDto[] SafetyCarActualCounts, SeasonSafetyCarStatsDto[] SafetyCarCorrectGuesses);
