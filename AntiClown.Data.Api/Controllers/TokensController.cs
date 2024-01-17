using AntiClown.Data.Api.Core.Tokens.Services;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Data.Api.Controllers;

[Route("dataApi/tokens/{userId:guid}")]
public class TokensController : Controller
{
    public TokensController(ITokensService tokensService)
    {
        this.tokensService = tokensService;
    }

    [HttpDelete]
    public async Task<ActionResult> InvalidateAsync([FromRoute] Guid userId)
    {
        await tokensService.InvalidateAsync(userId);
        return NoContent();
    }

    [HttpPost("validate")]
    public async Task<ActionResult> ValidateAsync([FromRoute] Guid userId, [FromBody] string token)
    {
        await tokensService.ValidateAsync(userId, token);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAsync([FromRoute] Guid userId)
    {
        return Json(await tokensService.GetAsync(userId));
    }

    private readonly ITokensService tokensService;
}