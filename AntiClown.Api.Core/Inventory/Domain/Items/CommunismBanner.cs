using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items;

public class CommunismBanner : BaseItem
{
    public int DivideChance { get; set; }
    public int StealChance { get; set; }
    public override ItemType ItemType => ItemType.Negative;
    public override ItemName ItemName => ItemName.CommunismBanner;
}