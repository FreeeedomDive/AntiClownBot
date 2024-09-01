using AntiClown.DiscordBot.Dto.Members;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.DiscordBot.Web.Controllers;

[ApiController]
[Route("discordApi/members")]
public class DiscordMembersController : Controller
{
    public DiscordMembersController(
        IUsersCache usersCache,
        IDiscordClientWrapper discordClientWrapper
    )
    {
        this.usersCache = usersCache;
        this.discordClientWrapper = discordClientWrapper;
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

    [HttpGet("findByRoleId")]
    public async Task<DiscordMemberDto?[]> Find([FromQuery] ulong roleId)
    {
        var membersIds = await discordClientWrapper.Roles.GetRoleMembersIdsAsync(roleId);
        var userIds = await Task.WhenAll(membersIds.Select(x => usersCache.GetApiIdByMemberIdAsync(x)));
        var members = await Task.WhenAll(userIds.Select(GetDiscordMemberAsync));
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
            UserId = userId,
            ServerName = discordMember.Nickname,
            UserName = discordMember.Username,
            AvatarUrl = discordMember.AvatarUrl,
        };
    }

    private readonly IUsersCache usersCache;
    private readonly IDiscordClientWrapper discordClientWrapper;
}