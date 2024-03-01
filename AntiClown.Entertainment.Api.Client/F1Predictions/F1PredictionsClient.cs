using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public class F1PredictionsClient : IF1PredictionsClient
{
    public F1PredictionsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<F1RaceDto> ReadAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<F1RaceDto>();
    }

    public async Task<Guid> StartNewRaceAsync(string name)
    {
        var request = new RestRequest("f1Predictions").AddQueryParameter("name", name);
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    public async Task ClosePredictionsAsync(Guid raceId)
    {
        var request = new RestRequest($"f1Predictions/{raceId}/close");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task<Dictionary<Guid, F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null)
    {
        var request = new RestRequest("f1Predictions/standings");
        if (season.HasValue)
        {
            request.AddQueryParameter("season", season.Value);
        }
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<Dictionary<Guid, F1PredictionUserResultDto?[]>>();
    }

    private readonly RestClient restClient;
}