using AntiClown.DiscordBot.Dto.Members;
using AntiClown.DiscordBot.Cache.Users;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.DiscordBot.Web.Controllers;

[ApiController]
[Route("discordApi/members")]
public class DiscordMembersController : Controller
{
    public DiscordMembersController(
        IUsersCache usersCache
    )
    {
        this.usersCache = usersCache;
    }

    [HttpGet("{userId:guid}")]
    public async Task<DiscordMemberDto?> GetDiscordMember([FromRoute] Guid userId)
    {
        return await GetDiscordMemberAsync(userId);
    }

    [HttpPost("getMany")]
    public async Task<DiscordMemberDto?[]> GetDiscordMembers([FromBody] Guid[] usersIds)
    {
        var tasks = usersIds.Select(GetDiscordMemberAsync);
        var members = await Task.WhenAll(tasks);
        return members;
    }

    private async Task<DiscordMemberDto?> GetDiscordMemberAsync(Guid userId)
    {
        var discordMember = await usersCache.GetMemberByApiIdAsync(userId);
        if (discordMember is null)
        {
            return null;
        }

        return new DiscordMemberDto
        {
            ServerName = discordMember.Nickname,
            UserName = discordMember.Username,
            AvatarUrl = discordMember.AvatarUrl,
        };
    }

    private readonly IUsersCache usersCache;
}