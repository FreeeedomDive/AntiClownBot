using RestSharp;

namespace AntiClown.DiscordApi.Client.Configuration;

public static class AntiClownDiscordApiClientProvider
{
    public static IAntiClownDiscordApiClient Build(string? baseApiUrl = "https://localhost:6325")
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri($"{baseApiUrl}/discordApi"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
        var restClient = new RestClient(restClientOptions);
        return new AntiClownDiscordApiClient(restClient);
    }
}