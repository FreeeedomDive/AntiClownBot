/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.GuessNumberEvent;

public class GuessNumberEventClient : IGuessNumberEventClient
{
    public GuessNumberEventClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberEventDto> ReadAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/guessNumber/{eventId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberEventDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> StartNewAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/guessNumber/start", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    public async Task AddPickAsync(System.Guid eventId, AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberUserPickDto guessNumberUserPickDto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/guessNumber/{eventId}/addPick", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(guessNumberUserPickDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task FinishAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/guessNumber/{eventId}/finish", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
