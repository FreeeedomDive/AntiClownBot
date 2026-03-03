using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Shops.Domain;

public class ShopItem
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public ItemName Name { get; set; }
    public Rarity Rarity { get; set; }
    public int Price { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsOwned { get; set; }
}