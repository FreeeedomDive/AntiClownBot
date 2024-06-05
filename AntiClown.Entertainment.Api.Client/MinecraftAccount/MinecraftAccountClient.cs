/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.MinecraftAccount;

public class MinecraftAccountClient : IMinecraftAccountClient
{
    public MinecraftAccountClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterResponse> RegisterAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterRequest registerRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAccount/register", Method.Post);
        request.AddJsonBody(registerRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterResponse>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinResponse> SetSkinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinRequest changeSkinRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAccount/setSkin", Method.Post);
        request.AddJsonBody(changeSkinRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinResponse>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.GetRegisteredUsersResponse> GetAllNicknamesAsync()
    {
        var request = new RestRequest("entertainmentApi/minecraftAccount/getNicknames", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.GetRegisteredUsersResponse>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasRegistrationResponse> HasRegistrationByDiscordUserAsync(System.Guid discordUserId)
    {
        var request = new RestRequest("entertainmentApi/minecraftAccount/hasRegistration/byDiscordUser/{discordUserId}", Method.Get);
        request.AddUrlSegment("discordUserId", discordUserId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasRegistrationResponse>();
    }

    private readonly RestSharp.RestClient restClient;
}
