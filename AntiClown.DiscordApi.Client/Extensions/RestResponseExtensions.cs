using AntiClown.Core.Dto.Exceptions;
using Newtonsoft.Json;
using RestSharp;

namespace AntiClown.DiscordApi.Client.Extensions;

internal static class RestResponseExtensions
{
    public static void ThrowIfNotSuccessful(this RestResponse restResponse)
    {
        if (restResponse.IsSuccessful)
        {
            return;
        }

        if (restResponse.Content == null)
        {
            throw new Exception("Content is null");
        }

        var knownApiException = JsonConvert.DeserializeObject<AntiClownBaseException>(restResponse.Content, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
        });
        throw knownApiException ?? new Exception("Unknown API error");
    }

    public static T TryDeserialize<T>(this RestResponse restResponse)
    {
        restResponse.ThrowIfNotSuccessful();
        if (restResponse.Content == null)
        {
            throw new Exception("Content is null");
        }

        try
        {
            Console.WriteLine(restResponse.Content);
            var response = JsonConvert.DeserializeObject<T>(restResponse.Content, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            });
            if (response == null)
            {
                throw new Exception($"Can not deserialize response as {typeof(T).Name}");
            }

            return response;
        }
        catch
        {
            throw new Exception($"Can not deserialize response as {typeof(T).Name}");
        }
    }
}