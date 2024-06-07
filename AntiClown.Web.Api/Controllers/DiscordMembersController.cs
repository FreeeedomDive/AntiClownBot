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

    private readonly IAntiClownDiscordBotClient antiClownDiscordBotClient;
}