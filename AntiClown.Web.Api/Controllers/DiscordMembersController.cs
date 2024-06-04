using AntiClown.DiscordBot.Client;
using AntiClown.DiscordApi.Dto.Members;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/discordMembers")]
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