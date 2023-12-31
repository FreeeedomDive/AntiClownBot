using RestSharp;

namespace RestSharpClient;

public static class RestClientBuilder
{
    public static RestClient BuildRestClient(string url, bool validateSsl = true)
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(url)
        };
        if (!validateSsl)
        {
            restClientOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }
        return new RestClient(restClientOptions);
    }
}