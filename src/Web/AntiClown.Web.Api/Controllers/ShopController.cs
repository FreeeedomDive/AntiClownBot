using AntiClown.Api.Client;
using AntiClown.Api.Dto.Inventories;
using AntiClown.Api.Dto.Shops;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/shops/{userId:guid}")]
[RequireUserToken]
public class ShopController(IAntiClownApiClient antiClownApiClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CurrentShopInfoDto>> GetShop([FromRoute] Guid userId)
    {
        return await antiClownApiClient.Shop.ReadAsync(userId);
    }

    [HttpPost("items/{itemId:guid}/reveal")]
    public async Task<ActionResult<ShopItemDto>> RevealItem([FromRoute] Guid userId, [FromRoute] Guid itemId)
    {
        return await antiClownApiClient.Shop.RevealAsync(userId, itemId);
    }

    [HttpPost("items/{itemId:guid}/buy")]
    public async Task<ActionResult<BaseItemDto>> BuyItem([FromRoute] Guid userId, [FromRoute] Guid itemId)
    {
        return await antiClownApiClient.Shop.BuyAsync(userId, itemId);
    }

    [HttpPost("reroll")]
    public async Task<ActionResult> ReRoll([FromRoute] Guid userId)
    {
        await antiClownApiClient.Shop.ReRollAsync(userId);
        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ShopStatsDto>> GetStats([FromRoute] Guid userId)
    {
        return await antiClownApiClient.Shop.ReadStatsAsync(userId);
    }
}
