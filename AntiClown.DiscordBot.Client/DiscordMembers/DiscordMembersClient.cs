/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.DiscordBot.Client.DiscordMembers;

public class DiscordMembersClient : IDiscordMembersClient
{
    public DiscordMembersClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto> GetDiscordMemberAsync(System.Guid userId)
    {
        var request = new RestRequest("discordApi/members/{userId}", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto>();
    }

    private readonly RestSharp.RestClient restClient;
}
