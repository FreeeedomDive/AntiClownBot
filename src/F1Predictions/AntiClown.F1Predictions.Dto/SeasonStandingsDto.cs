namespace AntiClown.F1Predictions.Dto;

public record SeasonRacePointsDto(Guid RaceId, PredictionPointsDto? Points);
public record SeasonPodiumDto(int Season, int Place);
public record SeasonStandingsRowDto(Guid ParticipantId, string ParticipantName, int TotalPoints,
    int MaximumTenthPlaceCount, SeasonRacePointsDto[] Results, SeasonPodiumDto[] PreviousPodiums);
public record SeasonParticipantChartDto(Guid ParticipantId, int[] Points);
public record SeasonStandingsDto(int Season, RaceDto[] Events, SeasonStandingsRowDto[] Rows,
    Guid? CurrentLeaderParticipantId, int CurrentLeaderPoints, int PointsLeft,
    SeasonParticipantChartDto[] Charts, int[] ChampionChart);
