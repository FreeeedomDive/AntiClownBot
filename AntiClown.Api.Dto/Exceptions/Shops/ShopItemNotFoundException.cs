using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Shops;

public class ShopItemNotFoundException : AntiClownApiNotFoundException
{
    public ShopItemNotFoundException(Guid shopId, Guid itemId)
        : base($"Item {itemId} not found in shop {shopId}")
    {
        ShopId = shopId;
        ItemId = itemId;
    }
    
    public Guid ShopId { get; set; }
    public Guid ItemId { get; set; }
}