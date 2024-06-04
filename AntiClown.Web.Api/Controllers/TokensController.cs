using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/tokens/{userId:guid}")]
public class TokensController : Controller
{
    public TokensController(IAntiClownDataApiClient antiClownDataApiClient)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    [HttpPost("validate")]
    public async Task<ActionResult> ValidateAsync([FromRoute] Guid userId, [FromBody] string token)
    {
        await antiClownDataApiClient.Tokens.ValidateAsync(userId, new TokenDto
        {
            Token = token,
        });
        return NoContent();
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}