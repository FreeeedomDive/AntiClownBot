using AntiClown.DiscordBot.Client;
using AntiClown.DiscordBot.Dto.Members;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/discordMembers")]
[RequireUserToken]
public class DiscordMembersController : Controller
{
    public DiscordMembersController(IAntiClownDiscordBotClient antiClownDiscordBotClient)
    {
        this.antiClownDiscordBotClient = antiClownDiscordBotClient;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<DiscordMemberDto?>> GetDiscordMember(Guid userId)
    {
        return await antiClownDiscordBotClient.DiscordMembers.GetDiscordMemberAsync(userId);
    }

    [HttpPost("getMany")]
    public async Task<DiscordMemberDto?[]> GetDiscordMembers([FromBody] Guid[] usersIds)
    {
        return await antiClownDiscordBotClient.DiscordMembers.GetDiscordMembersAsync(usersIds);
    }

    private readonly IAntiClownDiscordBotClient antiClownDiscordBotClient;
}