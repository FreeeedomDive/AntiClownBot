using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Shops;

public class ShopItemAlreadyBoughtException : AntiClownApiConflictException
{
    public ShopItemAlreadyBoughtException(Guid shopId, Guid itemId)
        : base($"Item {itemId} is already bought in shop {shopId}")
    {
        ShopId = shopId;
        ItemId = itemId;
    }
    
    public Guid ShopId { get; set; }
    public Guid ItemId { get; set; }
}