using RestSharp;

namespace RestSharpClient.Extensions;

public static class RestClientExtensions
{
    public static async Task<RestResponse> ExecuteDeleteAsync(this RestClient restClient, RestRequest request)
    {
        return await restClient.ExecuteAsync(request, Method.Delete);
    }
}