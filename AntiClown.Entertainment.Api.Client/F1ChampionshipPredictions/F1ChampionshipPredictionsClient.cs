/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.F1ChampionshipPredictions;

public class F1ChampionshipPredictionsClient : IF1ChampionshipPredictionsClient
{
    public F1ChampionshipPredictionsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto> ReadAsync(System.Guid userId, int season)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1ChampionshipPredictions", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("userId", userId);
        requestBuilder.WithQueryParameter("season", season);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto>(requestBuilder.Build());
    }

    public async Task CreateOrUpdateAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto dto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1ChampionshipPredictions", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(dto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipResultsDto> ReadResultsAsync(int season)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1ChampionshipPredictions/results", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("season", season);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipResultsDto>(requestBuilder.Build());
    }

    public async Task WriteResultsAsync(int season, AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipResultsDto dto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1ChampionshipPredictions/results", HttpRequestMethod.POST);
        requestBuilder.WithQueryParameter("season", season);
        requestBuilder.WithJsonBody(dto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
