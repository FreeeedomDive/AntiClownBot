/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public class MinecraftAuthClient : IMinecraftAuthClient
{
    public MinecraftAuthClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthResponseDto> AuthAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthRequest authRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAuth/auth", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(authRequest);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthResponseDto>(requestBuilder.Build());
    }

    public async Task<bool> JoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.JoinRequest joinRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAuth/join", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(joinRequest);
        return await client.MakeRequestAsync<bool>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinedResponseDto> HasJoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinRequest hasJoinRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAuth/hasJoin", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(hasJoinRequest);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinedResponseDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileResponseDto> ProfileAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileRequest profileRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAuth/profile", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(profileRequest);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileResponseDto>(requestBuilder.Build());
    }

    public async Task<IEnumerable<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesResponseDto>> ProfilesAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesRequest profilesRequest)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/minecraftAuth/profiles", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(profilesRequest);
        return await client.MakeRequestAsync<IEnumerable<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesResponseDto>>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
