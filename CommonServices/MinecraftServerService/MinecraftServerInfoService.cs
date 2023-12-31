using Dto.MinecraftServerDto;
using RestSharpClient;
using RestSharp;

namespace CommonServices.MinecraftServerService;

public class MinecraftServerInfoService : IMinecraftServerInfoService
{
    public async Task<MinecraftServerInfo?> ReadServerInfo(string serverAddress)
    {
        var restClient = RestClientBuilder.BuildRestClient("https://api.mcsrvstat.us/2");
        var request = new RestRequest(serverAddress);

        var response = await restClient.ExecuteGetAsync<MinecraftServerInfo>(request);
        return !response.IsSuccessful ? null : response.Data;
    }
}