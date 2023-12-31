using Dto.MinecraftServerDto;
using RestSharpClient;
using RestSharp;

namespace CommonServices.IpService;

public class IpService : IIpService
{
    public async Task<string?> GetIp()
    {
        var restClient = RestClientBuilder.BuildRestClient("https://api.ipify.org");

        var response = await restClient.ExecuteGetAsync(new RestRequest());
        return !response.IsSuccessful ? null : response.Content;
    }
}