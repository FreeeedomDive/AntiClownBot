using AntiClown.Data.Api.Client;
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
        Console.WriteLine(token);
        await antiClownDataApiClient.Tokens.ValidateAsync(userId, token);
        return NoContent();
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}