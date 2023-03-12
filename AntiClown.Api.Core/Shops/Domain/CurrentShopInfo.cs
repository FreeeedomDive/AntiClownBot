namespace AntiClown.Api.Core.Shops.Domain;

public class CurrentShopInfo
{
    public Guid Id { get; set; }
    public int ReRollPrice { get; set; }
    public int FreeReveals { get; set; }
    public ShopItem[] Items { get; set; }
}