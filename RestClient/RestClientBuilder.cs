using RestSharp;

namespace RestClient;

public static class RestClientBuilder
{
    public static RestSharp.RestClient BuildRestClient(string url)
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(url)
        };
        return new RestSharp.RestClient(restClientOptions);
    }
}