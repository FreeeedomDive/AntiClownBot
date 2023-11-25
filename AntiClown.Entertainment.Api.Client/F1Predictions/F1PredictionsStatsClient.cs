using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public class F1PredictionsStatsClient : IF1PredictionsStatsClient
{
    public F1PredictionsStatsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<MostPickedDriversStatsDto> GetMostPickedDriversAsync()
    {
        var request = new RestRequest("f1Predictions/stats/mostPickedDrivers");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<MostPickedDriversStatsDto>();
    }

    public async Task<MostPickedDriversStatsDto> GetMostPickedDriversAsync(Guid userId)
    {
        var request = new RestRequest($"f1Predictions/stats/{userId}/mostPickedDrivers");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<MostPickedDriversStatsDto>();
    }

    public async Task<MostProfitableDriversStatsDto> GetMostProfitableDriversAsync()
    {
        var request = new RestRequest("f1Predictions/stats/mostProfitableDrivers");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<MostProfitableDriversStatsDto>();
    }

    private readonly RestClient restClient;
}