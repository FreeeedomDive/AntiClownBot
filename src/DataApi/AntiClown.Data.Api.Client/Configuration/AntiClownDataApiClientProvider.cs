using RestSharp;

namespace AntiClown.Data.Api.Client.Configuration;

public static class AntiClownDataApiClientProvider
{
    public static IAntiClownDataApiClient Build(string baseApiUrl = "https://localhost:7206")
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(baseApiUrl),
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
        var restClient = new RestClient(restClientOptions);
        return new AntiClownDataApiClient(restClient);
    }
}