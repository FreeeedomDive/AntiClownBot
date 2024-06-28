/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.DiscordBot.Client.DiscordMembers;

public class DiscordMembersClient : IDiscordMembersClient
{
    public DiscordMembersClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto> GetDiscordMemberAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"discordApi/members/{userId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
