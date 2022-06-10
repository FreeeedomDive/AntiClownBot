namespace AntiClownDiscordBotVersion2.Models.Shop;

public interface IShopService
{
    void Create(ulong userId, Shop shop);
    bool TryRead(ulong userId, out Shop shop);
}