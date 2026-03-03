using AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.MinecraftAuth;

[Route("entertainmentApi/minecraftAccount")]
[ApiController]
public class MinecraftAccountController : ControllerBase
{
    public MinecraftAccountController(
        IMinecraftAccountService minecraftAccountService,
        IMapper mapper
    )
    {
        this.minecraftAccountService = minecraftAccountService;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        var authStatus = await minecraftAccountService.CreateOrChangeAccountAsync(
            registerRequest.DiscordId,
            registerRequest.Username,
            registerRequest.Password
        );

        return new RegisterResponse
        {
            SuccessfulStatus = mapper.Map<RegistrationStatusDto>(authStatus)
        };
    }

    [HttpPost("setSkin")]
    public async Task<ActionResult<ChangeSkinResponse>> SetSkin([FromBody] ChangeSkinRequest changeSkinRequest)
    {
        var result =
            await minecraftAccountService.SetSkinAsync(changeSkinRequest.DiscordUserId, changeSkinRequest.SkinUrl, changeSkinRequest.CapeUrl);

        return new ChangeSkinResponse
        {
            IsSuccess = result
        };
    }

    [HttpGet("getNicknames")]
    public async Task<ActionResult<GetRegisteredUsersResponse>> GetAllNicknames()
    {
        var nicknames = await minecraftAccountService.GetAllNicknames();

        return new GetRegisteredUsersResponse
        {
            Usernames = nicknames
        };
    }

    [HttpGet("hasRegistration/byDiscordUser/{discordUserId:guid}")]
    public async Task<ActionResult<HasRegistrationResponse>> HasRegistrationByDiscordUser([FromRoute] Guid discordUserId)
    {
        return new HasRegistrationResponse
        {
            HasRegistration = await minecraftAccountService.HasRegistrationByDiscordUser(discordUserId)
        };
    }

    private readonly IMapper mapper;
    private readonly IMinecraftAccountService minecraftAccountService;
}