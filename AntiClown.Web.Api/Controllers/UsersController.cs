using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Core.Dto.Exceptions;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/users")]
[RequireUserToken]
public class UsersController(IAntiClownApiClient antiClownApiClient) : Controller
{
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
}