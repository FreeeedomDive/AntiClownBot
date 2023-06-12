using RestSharp;

namespace AntiClown.EntertainmentApi.Client.Configuration;

public static class ClientProvider
{
    public static IAntiClownEntertainmentApiClient Build(string baseApiUrl = "https://localhost:7088")
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri($"{baseApiUrl}/entertainmentApi"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        };
        var restClient = new RestClient(restClientOptions);
        return new AntiClownEntertainmentApiClient(restClient);
    }
}