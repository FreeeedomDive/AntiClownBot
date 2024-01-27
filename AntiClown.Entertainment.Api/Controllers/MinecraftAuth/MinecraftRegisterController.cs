using AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.MinecraftAuth;

[Route("entertainmentApi/minecraftRegister")]
[ApiController]
public class MinecraftRegisterController : ControllerBase
{
    private readonly IMinecraftRegisterService minecraftRegisterService;

    public MinecraftRegisterController(
        IMinecraftRegisterService minecraftRegisterService
    )
    {
        this.minecraftRegisterService = minecraftRegisterService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromBody] RegisterRequest request)
    {
        var isSuccessful = await minecraftRegisterService.CreateOrChangeAccountAsync(
            request.DiscordId,
            request.Username,
            request.Password
        );

        return new RegisterResponse
        {
            IsSuccessful = isSuccessful
        };
    }
}