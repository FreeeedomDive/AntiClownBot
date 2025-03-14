﻿using RestSharp;

namespace AntiClown.DiscordBot.Client.Configuration;

public static class AntiClownDiscordApiClientProvider
{
    public static IAntiClownDiscordBotClient Build(string baseApiUrl = "https://localhost:6325")
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(baseApiUrl),
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
        var restClient = new RestClient(restClientOptions);
        return new AntiClownDiscordBotClient(restClient);
    }
}