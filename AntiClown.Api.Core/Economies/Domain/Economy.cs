using AntiClown.Api.Core.Common;
using AntiClown.Api.Core.Inventory.Domain;

namespace AntiClown.Api.Core.Economies.Domain;

public class Economy
{
    // equal to UserId
    public Guid Id { get; set; }
    public int ScamCoins { get; set; }
    public DateTime NextTribute { get; set; }
    public int LootBoxes { get; set; }

    public bool IsTributeReady()
    {
        return DateTime.UtcNow > NextTribute;
    }

    public static readonly Economy Default = new()
    {
        ScamCoins = Constants.DefaultScamCoins,
        NextTribute = DateTime.UtcNow,
        LootBoxes = 0,
    };
}