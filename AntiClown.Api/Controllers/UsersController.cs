﻿using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Services;
using AntiClown.Api.Dto.Users;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/users")]
public class UsersController : Controller
{
    public UsersController(
        IUsersService usersService,
        INewUserService newUserService,
        IMapper mapper
    )
    {
        this.usersService = usersService;
        this.newUserService = newUserService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<UserDto[]>> ReadAll()
    {
        var result = await usersService.ReadAllAsync();
        return mapper.Map<UserDto[]>(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> Read([FromRoute] Guid userId)
    {
        var result = await usersService.ReadAsync(userId);
        return mapper.Map<UserDto>(result);
    }

    [HttpPost("find")]
    public async Task<ActionResult<UserDto[]>> Find([FromBody] UserFilterDto filter)
    {
        var result = await usersService.FindAsync(mapper.Map<UserFilter>(filter));
        return mapper.Map<UserDto[]>(result);
    }

    [HttpPost("integrations/find")]
    public async Task<ActionResult<FindByIntegrationResultDto>> FindByIntegration([FromBody] UserIntegrationFilterDto filter)
    {
        var result = await usersService.FindByIntegrationIdAsync(mapper.Map<UserIntegrationFilter>(filter));
        return new FindByIntegrationResultDto
        {
            User = result is null ? null : mapper.Map<UserDto>(result),
        };
    }

    [HttpPost("integrations")]
    public async Task<ActionResult> CreateIntegration([FromBody] CreateCustomIntegrationDto createCustomIntegration)
    {
        await usersService.CreateOrUpdateCustomIntegration(createCustomIntegration.UserId, createCustomIntegration.IntegrationName, createCustomIntegration.IntegrationUserId);
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] NewUserDto newUser)
    {
        return await newUserService.CreateNewUserAsync(mapper.Map<NewUser>(newUser));
    }

    [HttpPatch("{userId:guid}/bindTelegram")]
    public async Task BindTelegramAsync([FromRoute] Guid userId, [FromQuery] long telegramId)
    {
        await usersService.BindTelegramUserIdAsync(userId, telegramId);
    }

    private readonly IMapper mapper;
    private readonly INewUserService newUserService;
    private readonly IUsersService usersService;
}