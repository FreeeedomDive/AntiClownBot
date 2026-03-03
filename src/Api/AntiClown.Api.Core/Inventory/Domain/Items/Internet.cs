using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items;

public class Internet : BaseItem
{
    public int Gigabytes { get; set; }
    public int Speed { get; set; }
    public int Ping { get; set; }
    public override ItemType ItemType => ItemType.Positive;
    public override ItemName ItemName => ItemName.Internet;
}