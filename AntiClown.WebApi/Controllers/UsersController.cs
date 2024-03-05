using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Core.Dto.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.WebApi.Controllers;

[Route("webApi/users")]
public class UsersController : Controller
{
    public UsersController(IAntiClownApiClient antiClownApiClient)
    {
        this.antiClownApiClient = antiClownApiClient;
    }

    [HttpGet("{userId:guid}")]
    public async Task<UserDto?> Read([FromRoute] Guid userId)
    {
        try
        {
            return await antiClownApiClient.Users.ReadAsync(userId);
        }
        catch (EntityNotFoundException)
        {
            return null;
        }
    }

    private readonly IAntiClownApiClient antiClownApiClient;
}