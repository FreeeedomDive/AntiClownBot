using AntiClown.Api.Client;
using AntiClown.Api.Dto.Inventories;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/inventories/{userId:guid}")]
[RequireUserToken]
public class InventoryController(IAntiClownApiClient antiClownApiClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<InventoryDto>> GetInventory([FromRoute] Guid userId)
    {
        return await antiClownApiClient.Inventory.ReadAllAsync(userId);
    }

    [HttpPost("items/{itemId:guid}/active/{isActive:bool}")]
    public async Task<ActionResult> ChangeActiveStatus(
        [FromRoute] Guid userId,
        [FromRoute] Guid itemId,
        [FromRoute] bool isActive
    )
    {
        await antiClownApiClient.Inventory.ChangeItemActiveStatusAsync(userId, itemId, isActive);
        return NoContent();
    }

    [HttpPost("items/{itemId:guid}/sell")]
    public async Task<ActionResult> SellItem([FromRoute] Guid userId, [FromRoute] Guid itemId)
    {
        await antiClownApiClient.Inventory.SellAsync(userId, itemId);
        return NoContent();
    }
}
