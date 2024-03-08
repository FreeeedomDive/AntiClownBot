using AntiClown.Core.Dto.Extensions;
using AntiClown.DiscordApi.Dto.Members;
using RestSharp;

namespace AntiClown.DiscordApi.Client.Members;

public class MembersClient : IMembersClient
{
    public MembersClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<DiscordMemberDto?> GetDiscordMemberAsync(Guid userId)
    {
        var request = new RestRequest($"members/{userId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<DiscordMemberDto?>();
    }

    private readonly RestClient restClient;
}