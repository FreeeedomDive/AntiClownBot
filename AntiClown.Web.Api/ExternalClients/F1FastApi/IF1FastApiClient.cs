using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Web.Api.ExternalClients.F1FastApi;

public interface IF1FastApiClient
{
    Task<F1PredictionRaceResultDto> GetF1PredictionRaceResult(Guid raceId, bool isSprint);
}