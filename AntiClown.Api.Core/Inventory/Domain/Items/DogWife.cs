using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items;

public class DogWife : BaseItem
{
    public int LootBoxFindChance { get; set; }
    public override ItemType ItemType => ItemType.Positive;
    public override ItemName ItemName => ItemName.DogWife;
}