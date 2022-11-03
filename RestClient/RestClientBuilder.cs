using RestSharp;

namespace RestClient;

public static class RestClientBuilder
{
    public static RestSharp.RestClient BuildRestClient(string url, bool validateSsl = true)
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(url)
        };
        if (!validateSsl)
        {
            restClientOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }
        return new RestSharp.RestClient(restClientOptions);
    }
}