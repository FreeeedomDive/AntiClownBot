using AntiClown.Api.Dto.Shops;

namespace AntiClown.DiscordBot.Interactivity.Domain.Shop;

public class ShopDetails
{
    public Guid UserId { get; set; }
    public ShopTool Tool { get; set; }
    public CurrentShopInfoDto Shop { get; set; }
    public Dictionary<int, Guid> BoughtItems { get; set; }
}