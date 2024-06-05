/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public class MinecraftAuthClient : IMinecraftAuthClient
{
    public MinecraftAuthClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthResponseDto> AuthAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthRequest authRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAuth/auth", Method.Post);
        request.AddJsonBody(authRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthResponseDto>();
    }

    public async System.Threading.Tasks.Task<System.Boolean> JoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.JoinRequest joinRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAuth/join", Method.Post);
        request.AddJsonBody(joinRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Boolean>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinedResponseDto> HasJoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinRequest hasJoinRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAuth/hasJoin", Method.Post);
        request.AddJsonBody(hasJoinRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinedResponseDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileResponseDto> ProfileAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileRequest profileRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAuth/profile", Method.Post);
        request.AddJsonBody(profileRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileResponseDto>();
    }

    public async System.Threading.Tasks.Task<IEnumerable<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesResponseDto>> ProfilesAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesRequest profilesRequest)
    {
        var request = new RestRequest("entertainmentApi/minecraftAuth/profiles", Method.Post);
        request.AddJsonBody(profilesRequest);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<IEnumerable<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesResponseDto>>();
    }

    private readonly RestSharp.RestClient restClient;
}
