using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;

public class ChampionshipPredictionsClosedException : ConflictException
{
    public ChampionshipPredictionsClosedException(int season) : base($"Championship predictions are closed for season {season}")
    {
        Season = season;
    }

    public int Season { get; }
}
