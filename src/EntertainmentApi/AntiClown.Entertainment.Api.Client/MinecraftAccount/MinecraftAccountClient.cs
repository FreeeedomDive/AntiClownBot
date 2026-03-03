/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.MinecraftAccount;

public class MinecraftAccountClient : IMinecraftAccountClient
{
    public MinecraftAccountClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterResponse> RegisterAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterRequest registerRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAccount/register", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(registerRequest);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterResponse>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinResponse> SetSkinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinRequest changeSkinRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAccount/setSkin", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(changeSkinRequest);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinResponse>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.GetRegisteredUsersResponse> GetAllNicknamesAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAccount/getNicknames", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.GetRegisteredUsersResponse>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasRegistrationResponse> HasRegistrationByDiscordUserAsync(System.Guid discordUserId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAccount/hasRegistration/byDiscordUser/{discordUserId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasRegistrationResponse>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
