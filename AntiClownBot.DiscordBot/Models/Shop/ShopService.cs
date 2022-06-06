namespace AntiClownDiscordBotVersion2.Models.Shop;

public class ShopService : IShopService
{
    public ShopService()
    {
        shops = new Dictionary<ulong, Shop>();
    }

    public void Create(ulong userId, Shop shop)
    {
        shops[userId] = shop;
    }

    public bool TryRead(ulong userId, out Shop shop)
    {
        shop = null;
        if (!shops.ContainsKey(userId))
        {
            return false;
        }

        shop = shops[userId];
        return true;
    }

    private readonly Dictionary<ulong, Shop> shops;
}