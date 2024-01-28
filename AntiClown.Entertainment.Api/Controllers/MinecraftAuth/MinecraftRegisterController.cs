using AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.MinecraftAuth;

[Route("entertainmentApi/minecraftRegister")]
[ApiController]
public class MinecraftRegisterController : ControllerBase
{
    public MinecraftRegisterController(
        IMinecraftRegisterService minecraftRegisterService,
        IMapper mapper
    )
    {
        this.minecraftRegisterService = minecraftRegisterService;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromBody] RegisterRequest request)
    {
        var authStatus = await minecraftRegisterService.CreateOrChangeAccountAsync(
            request.DiscordId,
            request.Username,
            request.Password
        );

        return new RegisterResponse
        {
            SuccessfulStatus = mapper.Map<RegistrationStatusDto>(authStatus)
        };
    }

    private readonly IMinecraftRegisterService minecraftRegisterService;
    private readonly IMapper mapper;
}