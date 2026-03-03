namespace AntiClown.Api.Dto.Inventories;

public class BaseItemDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public RarityDto Rarity { get; set; }
    public int Price { get; set; }
    public bool IsActive { get; set; }
    public ItemTypeDto ItemType { get; set; }
    public ItemNameDto ItemName { get; set; }
}