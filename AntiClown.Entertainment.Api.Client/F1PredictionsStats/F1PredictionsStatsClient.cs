/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.F1PredictionsStats;

public class F1PredictionsStatsClient : IF1PredictionsStatsClient
{
    public F1PredictionsStatsClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync()
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/stats/mostPickedDrivers", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync(System.Guid userId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/stats/{userId}/mostPickedDrivers", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostProfitableDriversStatsDto> GetMostProfitableDriversAsync()
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/stats/mostProfitableDrivers", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostProfitableDriversStatsDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.UserPointsStatsDto> GetUserPointsStatsAsync(System.Guid userId)
    {
        var request = new RestRequest("entertainmentApi/f1Predictions/stats/{userId}/userPointsStats", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.UserPointsStatsDto>();
    }

    private readonly RestSharp.RestClient restClient;
}
