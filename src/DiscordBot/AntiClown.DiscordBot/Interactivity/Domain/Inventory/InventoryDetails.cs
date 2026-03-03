namespace AntiClown.DiscordBot.Interactivity.Domain.Inventory;

public class InventoryDetails
{
    public Guid UserId { get; set; }
    public InventoryPage[] Pages { get; set; }
    public int CurrentPage { get; set; }
    public InventoryTool Tool { get; set; }
}