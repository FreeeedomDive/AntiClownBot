using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Economies.Domain;

public class LootBoxReward
{
    public int ScamCoinsReward { get; set; }
    public List<BaseItem> Items { get; set; } = new();
}