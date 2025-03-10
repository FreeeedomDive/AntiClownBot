/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.F1Bingo;

public class F1BingoClient : IF1BingoClient
{
    public F1BingoClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.F1BingoCardDto[]> ReadCardsAsync(int season)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Bingo/cards", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("season", season);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.F1BingoCardDto[]>(requestBuilder.Build());
    }

    public async Task CreateCardAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.CreateF1BingoCardDto dto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Bingo/cards", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(dto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task UpdateCardAsync(System.Guid cardId, AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.UpdateF1BingoCardDto dto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Bingo/cards/{cardId}", HttpRequestMethod.PATCH);
        requestBuilder.WithJsonBody(dto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<System.Guid[]> GetBoardAsync(System.Guid userId, int season)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/f1Bingo/boards/{userId}", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("season", season);
        return await client.MakeRequestAsync<System.Guid[]>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
