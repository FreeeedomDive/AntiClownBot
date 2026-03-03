using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Interactivity.Domain.Inventory;

public class InventoryPage
{
    public ItemNameDto ItemName { get; set; }
    public string PageDescription { get; set; }
    public Guid[] ItemsIdsOnPage { get; set; }
}