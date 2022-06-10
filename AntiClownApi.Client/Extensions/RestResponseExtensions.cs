using AntiClownApiClient.Dto.Exceptions;
using AntiClownApiClient.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace AntiClownApiClient.Extensions;

public static class RestResponseExtensions
{
    public static void ThrowIfNotSuccessful(this RestResponse restResponse)
    {
        if (!restResponse.IsSuccessful)
        {
            throw new ApiException(restResponse.Content ?? "Server returned unsuccessful response");
        }
    }

    public static T TryDeserialize<T>(this RestResponse restResponse)
    {
        restResponse.ThrowIfNotSuccessful();
        if (restResponse.Content == null)
        {
            throw new ResponseDeserializationException("Content is null");
        }

        try
        {
            var response = JsonConvert.DeserializeObject<T>(restResponse.Content, new ItemConverter());
            if (response == null)
            {
                throw new ResponseDeserializationException($"Can not deserialize response as {typeof(T).Name}");
            }

            return response;
        }
        catch
        {
            throw new ResponseDeserializationException($"Can not deserialize response as {typeof(T).Name}");
        }
    }
}