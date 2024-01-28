using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public class MinecraftAccountClient : IMinecraftAccountClient
{
    public MinecraftAccountClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<RegistrationStatusDto> Register(RegisterRequest request)
    {
        var clientRequest = new RestRequest($"{ControllerUrl}/register");
        clientRequest.AddJsonBody(request);
        var response = await restClient.ExecutePostAsync(clientRequest);
        return response.TryDeserialize<RegisterResponse>().SuccessfulStatus;
    }

    public async Task<bool> SetSkinAsync(ChangeSkinRequest request)
    {
        var clientRequest = new RestRequest($"{ControllerUrl}/setSkin");
        clientRequest.AddJsonBody(request);
        var response = await restClient.ExecutePostAsync(clientRequest);
        return response.TryDeserialize<ChangeSkinResponse>().IsSuccess;
    }

    public async Task<string[]> GetAllNicknames()
    {
        var clientRequest = new RestRequest($"{ControllerUrl}/getNicknames");
        var response = await restClient.ExecuteGetAsync(clientRequest);
        return response.TryDeserialize<GetRegisteredUsersResponse>().Usernames;
    }

    public async Task<bool> HasRegistrationByDiscordUser(Guid discordUserId)
    {
        var clientRequest = new RestRequest($"{ControllerUrl}/hasRegistration/byDiscordUser/{discordUserId}");
        var response = await restClient.ExecuteGetAsync(clientRequest);
        return response.TryDeserialize<HasRegistrationResponse>().HasRegistration;
    }

    private readonly RestClient restClient;
    private const string ControllerUrl = "minecraftAccount";
}