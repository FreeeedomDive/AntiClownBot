using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Web.Api.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Web.Api.ExternalClients.F1FastApi;

public class F1FastApiClient : IF1FastApiClient
{
    public F1FastApiClient(IOptions<F1FastApiOptions> options, ILogger<F1FastApiClient> logger)
    {
        this.logger = logger;
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
        var request = new RestRequest(string.Empty)
                      .AddQueryParameter("raceId", raceId)
                      .AddQueryParameter("raceType", isSprint ? "Sprint" : "Race");
        var response = await restClient.ExecuteAsync(request);
        logger.LogInformation("F1FastApi Response:\n{response}", JsonConvert.SerializeObject(response, Formatting.Indented));
        return response.TryDeserialize<F1PredictionRaceResultDto>();
    }

    private readonly ILogger<F1FastApiClient> logger;

    private readonly RestClient restClient;
}