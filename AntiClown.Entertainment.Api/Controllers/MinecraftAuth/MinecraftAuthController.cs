using AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.MinecraftAuth;

[Route("entertainmentApi/minecraftAuth")]
[ApiController]
public class MinecraftAuthController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMinecraftAuthService minecraftAuthService;

    public MinecraftAuthController(
        IMinecraftAuthService minecraftAuthService,
        IMapper mapper
    )
    {
        this.minecraftAuthService = minecraftAuthService;
        this.mapper = mapper;
    }

    [HttpPost("auth")]
    public async Task<ActionResult<AuthResponseDto>> Auth([FromBody] AuthRequest request)
    {
        return mapper.Map<AuthResponseDto>(await minecraftAuthService.Auth(request.Username, request.Password));
    }

    [HttpPost("join")]
    public async Task<ActionResult<bool>> Join([FromBody] JoinRequest request)
    {
        return await minecraftAuthService.Join(request.AccessToken, request.UserUUID, request.ServerID);
    }

    [HttpPost("hasJoin")]
    public async Task<ActionResult<HasJoinedResponseDto>> HasJoin([FromBody] HasJoinRequest request)
    {
        return mapper.Map<HasJoinedResponseDto>(await minecraftAuthService.HasJoined(request.Username, request.ServerID));
    }

    [HttpPost("profile")]
    public async Task<ActionResult<ProfileResponseDto>> Profile([FromBody] ProfileRequest request)
    {
        return mapper.Map<ProfileResponseDto>(await minecraftAuthService.Profile(request.UserUUID));
    }

    [HttpPost("profiles")]
    public async Task<ActionResult<IEnumerable<ProfilesResponseDto>>> Profiles([FromBody] ProfilesRequest request)
    {
        var profiles = await minecraftAuthService.Profiles(request.Usernames);
        return Ok(profiles);
    }
}