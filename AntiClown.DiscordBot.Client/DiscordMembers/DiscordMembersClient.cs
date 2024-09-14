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

    public async Task<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto?[]> GetDiscordMembersAsync(System.Guid[] usersIds)
    {
        var requestBuilder = new RequestBuilder($"discordApi/members/getMany", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(usersIds);
        return await client.MakeRequestAsync<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto?[]> FindByRoleIdAsync(System.UInt64 roleId)
    {
        var requestBuilder = new RequestBuilder($"discordApi/members/findByRoleId", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("roleId", roleId);
        return await client.MakeRequestAsync<AntiClown.DiscordBot.Dto.Members.DiscordMemberDto[]>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
