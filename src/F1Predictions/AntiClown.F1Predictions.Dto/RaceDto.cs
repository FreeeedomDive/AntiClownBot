namespace AntiClown.F1Predictions.Dto;

public enum RaceTypeDto { RACE, SPRINT }
public enum RaceStatusDto { SCHEDULED, OPEN, CLOSED, FINISHED }
public enum SafetyCarsCountDto { ZERO, ONE, TWO, THREE_PLUS }
public enum RaceDriverStatusDto { FINISHED, DNF, DNS, DQ }

public record ListResponseDto<T>(T[] Data);
public record RaceDto(Guid Id, string Name, RaceTypeDto Type, RaceStatusDto Status, DateTimeOffset OpenAt, DateTimeOffset CloseAt);
public record TeamDto(string Name, string Badge, string BackgroundColor, string TextColor, string[] Drivers);
public record PredictionConditionsDto(string? PositionPredictionDriver);
public record RaceDetailsDto(RaceDto Race, int Season, TeamDto[] Teams, bool LineupPrepared,
    PredictionConditionsDto? Conditions, bool PredictionsOpen, DateTimeOffset ServerTime, string[] StartingGrid);
public record SavePredictionRequestDto(string? TenthPlacePickedDriver, string[]? DnfPickedDrivers,
    SafetyCarsCountDto? SafetyCarsPrediction, int? DriverPositionPrediction, decimal? FirstPlaceLeadPrediction);
public record PredictionDto(Guid Id, Guid LeagueId, Guid ParticipantId, Guid RaceId, string? TenthPlacePickedDriver,
    string[]? DnfPickedDrivers, SafetyCarsCountDto? SafetyCarsPrediction, int? DriverPositionPrediction,
    decimal? FirstPlaceLeadPrediction, string[]? TeamsPickedDrivers);
public record PredictionPointsDto(string RulesVersion, int TotalPoints, int? TenthPlacePoints, int? DnfsPoints,
    int? SafetyCarsPoints, int? FirstPlaceLeadPoints, int? TeamMatesPoints, int? DriverPositionPoints);
public record RaceClassificationEntryDto(string DriverName, RaceDriverStatusDto Status);
public record RaceResultValuesDto(RaceClassificationEntryDto[] Classification, int? SafetyCars, decimal? FirstPlaceLead);
public record RaceResultDetailsDto(RaceDto Race, int Season, string RulesVersion, RaceResultValuesDto Result, PredictionPointsDto? Points);
public record UserRaceDto(RaceDetailsDto Details, PredictionDto? Prediction, RaceResultDetailsDto? Result);
