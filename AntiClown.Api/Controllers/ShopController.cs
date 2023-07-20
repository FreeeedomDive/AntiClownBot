using AntiClown.Api.Core.Shops.Services;
using AntiClown.Api.Dto.Inventories;
using AntiClown.Api.Dto.Shops;
using AntiClown.Api.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/shops")]
public class ShopController : Controller
{
    public ShopController(
        IShopsService shopsService,
        IMapper mapper
    )
    {
        this.shopsService = shopsService;
        this.mapper = mapper;
    }

    [HttpGet("{shopId:guid}")]
    public async Task<ActionResult<CurrentShopInfoDto>> Read([FromRoute] Guid shopId)
    {
        var shop = await shopsService.ReadCurrentShopAsync(shopId);
        return mapper.Map<CurrentShopInfoDto>(shop);
    }

    [HttpPost("{shopId:guid}/items/{itemId:guid}/reveal")]
    public async Task<ActionResult<ShopItemDto>> Reveal([FromRoute] Guid shopId, [FromRoute] Guid itemId)
    {
        var item = await shopsService.RevealAsync(shopId, itemId);
        return mapper.Map<ShopItemDto>(item);
    }

    [HttpPost("{shopId:guid}/items/{itemId:guid}/buy")]
    public async Task<ActionResult<BaseItemDto>> Buy([FromRoute] Guid shopId, [FromRoute] Guid itemId)
    {
        var newItem = await shopsService.BuyAsync(shopId, itemId);
        return mapper.Map(newItem);
    }

    [HttpPost("{shopId:guid}/reroll")]
    public async Task<ActionResult> ReRoll([FromRoute] Guid shopId)
    {
        await shopsService.ReRollAsync(shopId);
        return NoContent();
    }

    [HttpPost("{shopId:guid}/reset")]
    public async Task<ActionResult> Reset([FromRoute] Guid shopId)
    {
        await shopsService.ResetShopAsync(shopId);
        return NoContent();
    }

    [HttpGet("{shopId:guid}/stats")]
    public async Task<ActionResult<ShopStatsDto>> ReadStats([FromRoute] Guid shopId)
    {
        var stats = await shopsService.ReadStatsAsync(shopId);
        return mapper.Map<ShopStatsDto>(stats);
    }

    [HttpPost("resetAll")]
    public async Task<ActionResult> ResetAll()
    {
        await shopsService.ResetAllAsync();
        return NoContent();
    }

    private readonly IMapper mapper;
    private readonly IShopsService shopsService;
}