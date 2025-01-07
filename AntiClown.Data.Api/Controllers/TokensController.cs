using AntiClown.Data.Api.Core.Tokens.Services;
using AntiClown.Data.Api.Dto.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Data.Api.Controllers;

[Route("dataApi/tokens/{userId:guid}")]
public class TokensController(ITokensService tokensService) : Controller
{
    [HttpDelete]
    public async Task<ActionResult> InvalidateAsync([FromRoute] Guid userId)
    {
        await tokensService.InvalidateAsync(userId);
        return NoContent();
    }

    [HttpPost("validate")]
    public async Task<ActionResult> ValidateAsync([FromRoute] Guid userId, [FromBody] TokenDto token)
    {
        await tokensService.ValidateAsync(userId, token.Token);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAsync([FromRoute] Guid userId)
    {
        return Json(await tokensService.GetAsync(userId));
    }
}