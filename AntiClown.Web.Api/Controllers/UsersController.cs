using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Core.Dto.Exceptions;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/users")]
[RequireUserToken]
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