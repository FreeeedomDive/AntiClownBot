using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Web.Api.Options;
using Microsoft.Extensions.Options;
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Web.Api.ExternalClients.F1FastApi;

public class F1FastApiClient : IF1FastApiClient
{
    public F1FastApiClient(IOptions<F1FastApiOptions> options)
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(options.Value.ServiceUrl),
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
            MaxTimeout = 1000 * 60 * 2,
        };
        restClient = new RestClient(restClientOptions);
    }

    public async Task<F1PredictionRaceResultDto> GetF1PredictionRaceResult(Guid raceId, bool isSprint)
    {
        var request = new RequestBuilder("", HttpRequestMethod.GET)
                      .WithQueryParameter("raceId", raceId)
                      .WithQueryParameter("raceType", isSprint ? "Sprint" : "Race")
                      .Build();
        return await restClient.MakeRequestAsync<F1PredictionRaceResultDto>(request);
    }

    private readonly RestClient restClient;
}