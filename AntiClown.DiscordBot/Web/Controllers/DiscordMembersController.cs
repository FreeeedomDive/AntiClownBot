using AntiClown.DiscordApi.Dto.Members;
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