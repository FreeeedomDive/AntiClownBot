namespace AntiClown.Api.Dto.Shops;

public class ShopItemDto
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public ItemNameDto Name { get; set; }
    public RarityDto Rarity { get; set; }
    public int Price { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsOwned { get; set; }
}