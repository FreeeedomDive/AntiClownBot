namespace AntiClownDiscordBotVersion2.Models.Inventory;

public interface IUserInventoryService
{
    void Create(ulong userId, UserInventory inventory);
    bool TryRead(ulong userId, out UserInventory inventory);
}