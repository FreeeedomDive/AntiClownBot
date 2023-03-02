using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items;

public class JadeRod : BaseItem
{
    public int Length { get; set; }
    public int Thickness { get; set; }
    public override ItemType ItemType => ItemType.Negative;
}