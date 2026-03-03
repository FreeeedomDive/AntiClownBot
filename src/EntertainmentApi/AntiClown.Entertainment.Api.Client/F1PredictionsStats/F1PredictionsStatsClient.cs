/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.F1PredictionsStats;

public class F1PredictionsStatsClient : IF1PredictionsStatsClient
{
    public F1PredictionsStatsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/stats/mostPickedDrivers", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/stats/{userId}/mostPickedDrivers", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostProfitableDriversStatsDto> GetMostProfitableDriversAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/stats/mostProfitableDrivers", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostProfitableDriversStatsDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.UserPointsStatsDto> GetUserPointsStatsAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/stats/{userId}/userPointsStats", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.UserPointsStatsDto>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
