namespace AntiClown.Api.Core.Inventory.Domain.Items.Base;

public abstract class BaseItem
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Rarity Rarity { get; set; }
    public int Price { get; set; }
    public bool IsActive { get; set; }
    public abstract ItemType ItemType { get; }
    public abstract ItemName ItemName { get; }
}