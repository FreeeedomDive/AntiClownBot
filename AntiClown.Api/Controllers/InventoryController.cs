using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Dto.Inventories;
using AntiClown.Api.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/inventories/{userId:guid}")]
public class InventoryController : Controller
{
    public InventoryController(
        IItemsService itemsService,
        IMapper mapper
    )
    {
        this.itemsService = itemsService;
        this.mapper = mapper;
    }

    [HttpGet("items")]
    public async Task<ActionResult<InventoryDto>> ReadAll([FromRoute] Guid userId)
    {
        var items = await itemsService.ReadAllItemsForUserAsync(userId);
        return mapper.MapInventory(items);
    }

    [HttpGet("items/{itemId:guid}")]
    public async Task<ActionResult<BaseItemDto>> Read([FromRoute] Guid userId, [FromRoute] Guid itemId)
    {
        var item = await itemsService.ReadItemAsync(userId, itemId);
        return mapper.Map(item);
    }

    [HttpPost("lootBoxes/open")]
    public async Task<ActionResult<LootBoxRewardDto>> OpenLootBox([FromRoute] Guid userId)
    {
        var reward = await itemsService.OpenLootBoxAsync(userId);
        return new LootBoxRewardDto
        {
            ScamCoinsReward = reward.ScamCoinsReward,
            Items = reward.Items.Select(x => x.Id).ToArray(),
        };
    }

    [HttpPatch("items/{itemId:guid}/active/{isActive:bool}")]
    public async Task<ActionResult> ChangeItemActiveStatus([FromRoute] Guid userId, [FromRoute] Guid itemId, [FromRoute] bool isActive)
    {
        await itemsService.ChangeItemActiveStatusAsync(userId, itemId, isActive);
        return NoContent();
    }

    [HttpPost("items/{itemId:guid}/sell")]
    public async Task<ActionResult> Sell([FromRoute] Guid userId, [FromRoute] Guid itemId)
    {
        await itemsService.SellItemAsync(userId, itemId);
        return NoContent();
    }

    private readonly IItemsService itemsService;
    private readonly IMapper mapper;
}