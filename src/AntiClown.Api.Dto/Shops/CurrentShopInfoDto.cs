namespace AntiClown.Api.Dto.Shops;

public class CurrentShopInfoDto
{
    public Guid Id { get; set; }
    public int ReRollPrice { get; set; }
    public int FreeReveals { get; set; }
    public ShopItemDto[] Items { get; set; }
}