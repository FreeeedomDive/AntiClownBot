/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public class F1PredictionsClient : IF1PredictionsClient
{
    public F1PredictionsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto[]> FindAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceFilterDto filter)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/find", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(filter);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto> ReadAsync(System.Guid raceId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/{raceId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> StartNewRaceAsync(string name, bool isSprint)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/", HttpRequestMethod.POST);
        requestBuilder.WithQueryParameter("name", name);
        requestBuilder.WithQueryParameter("isSprint", isSprint);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    public async Task AddPredictionAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionDto prediction)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/{raceId}/addPrediction", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(prediction);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task ClosePredictionsAsync(System.Guid raceId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/{raceId}/close", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task AddResultAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionRaceResultDto raceResult)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/{raceId}/addResult", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(raceResult);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> FinishRaceAsync(System.Guid raceId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/{raceId}/finish", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> ReadResultsAsync(System.Guid raceId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/{raceId}/results", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>(requestBuilder.Build());
    }

    public async Task<Dictionary<System.Guid, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>> ReadStandingsAsync(int? season = null)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/standings", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("season", season);
        return await client.MakeRequestAsync<Dictionary<System.Guid, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1TeamDto[]> ReadTeamsAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/teams", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.F1TeamDto[]>(requestBuilder.Build());
    }

    public async Task CreateOrUpdateTeamAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.F1TeamDto dto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Predictions/teams", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(dto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
