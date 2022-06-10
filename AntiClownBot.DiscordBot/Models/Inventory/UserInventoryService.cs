namespace AntiClownDiscordBotVersion2.Models.Inventory;

public class UserInventoryService : IUserInventoryService
{
    public UserInventoryService()
    {
        inventories = new Dictionary<ulong, UserInventory>();
    }
    
    public void Create(ulong userId, UserInventory inventory)
    {
        inventories[userId] = inventory;
    }

    public bool TryRead(ulong userId, out UserInventory inventory)
    {
        inventory = null;
        if (!inventories.ContainsKey(userId))
        {
            return false;
        }

        inventory = inventories[userId];
        return true;
    }

    private readonly Dictionary<ulong, UserInventory> inventories;
}