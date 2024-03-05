using AntiClown.DiscordApi.Client;
using AntiClown.DiscordApi.Dto.Members;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.WebApi.Controllers;

[Route("webApi/discordMembers")]
public class DiscordMembersController : Controller
{
    public DiscordMembersController(IAntiClownDiscordApiClient antiClownDiscordApiClient)
    {
        this.antiClownDiscordApiClient = antiClownDiscordApiClient;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<DiscordMemberDto?>> GetDiscordMember(Guid userId)
    {
        return await antiClownDiscordApiClient.MembersClient.GetDiscordMemberAsync(userId);
    }

    private readonly IAntiClownDiscordApiClient antiClownDiscordApiClient;
}