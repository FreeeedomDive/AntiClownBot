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

    public async Task<MostPickedDriversByUsersStatsDto> GetMostPickedDriversByUsersAsync()
    {
        var request = new RestRequest("f1Predictions/stats/mostPickedDriversByUsers");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<MostPickedDriversByUsersStatsDto>();
    }

    public async Task<MostProfitableDriversStatsDto> GetMostProfitableDriversAsync()
    {
        var request = new RestRequest("f1Predictions/stats/mostProfitableDrivers");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<MostProfitableDriversStatsDto>();
    }

    private readonly RestClient restClient;
}