using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.DiscordBot.Web.Controllers;

[ApiController]
[Route("discordApi/members")]
public class DiscordMembersController : Controller
{
    public DiscordMembersController(
        IDiscordClientWrapper discordClientWrapper,
        IUsersCache usersCache
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.usersCache = usersCache;
    }

    [HttpGet("{userId:guid}")]
    public async Task<DiscordMemberDto?> GetDiscordMember(Guid userId)
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

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IUsersCache usersCache;
}