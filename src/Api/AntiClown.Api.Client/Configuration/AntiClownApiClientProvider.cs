using RestSharp;

namespace AntiClown.Api.Client.Configuration;

public static class AntiClownApiClientProvider
{
    public static IAntiClownApiClient Build(string baseApiUrl = "https://localhost:7221")
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(baseApiUrl),
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
        var restClient = new RestClient(restClientOptions);
        return new AntiClownApiClient(restClient);
    }
}