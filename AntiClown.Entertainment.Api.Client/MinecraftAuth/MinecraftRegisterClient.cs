using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public class MinecraftRegisterClient : IMinecraftRegisterClient
{
    public MinecraftRegisterClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<bool> Register(RegisterRequest request)
    {
        var clientRequest = new RestRequest($"{ControllerUrl}/register");
        clientRequest.AddJsonBody(request);
        var response = await restClient.ExecutePostAsync(clientRequest);
        return response.TryDeserialize<RegisterResponse>().IsSuccessful;
    }

    private readonly RestClient restClient;
    private const string ControllerUrl = "minecraftRegister";
}