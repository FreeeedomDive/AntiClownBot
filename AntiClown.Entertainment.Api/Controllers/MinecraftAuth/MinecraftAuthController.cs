﻿using AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;
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
        var authResponse = await minecraftAuthService.Auth(request.Username, request.Password);
        if (authResponse is null)
            return Ok(new MinecraftErrorResponse
            {
                Error = new MinecraftErrorDto
                {
                    Code = 200,
                    Message = "Неверный логин или пароль"
                }
            });

        return mapper.Map<AuthResponseDto>(authResponse);
    }

    [HttpPost("join")]
    public async Task<ActionResult<bool>> Join([FromBody] JoinRequest request)
    {
        return await minecraftAuthService.Join(request.AccessToken, request.UserUUID, request.ServerID);
    }

    [HttpPost("hasJoin")]
    public async Task<ActionResult<HasJoinedResponseDto>> HasJoin([FromBody] HasJoinRequest request)
    {
        var hasJoinedResponse = await minecraftAuthService.HasJoined(request.Username, request.ServerID);
        if (hasJoinedResponse == null)
            throw new ArgumentException();

        return mapper.Map<HasJoinedResponseDto>(hasJoinedResponse);
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