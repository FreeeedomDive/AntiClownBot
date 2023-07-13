using AntiClown.Api.Core.Common;

namespace AntiClown.Api.Core.Economies.Domain;

public class Economy
{
    public bool IsTributeReady()
    {
        return DateTime.UtcNow > NextTribute;
    }

    // equal to UserId
    public Guid Id { get; set; }
    public int ScamCoins { get; set; }
    public DateTime NextTribute { get; set; }
    public int LootBoxes { get; set; }
    public bool IsLohotronReady { get; set; }

    public static readonly Economy Default = new()
    {
        ScamCoins = Constants.DefaultScamCoins,
        NextTribute = DateTime.UtcNow,
        LootBoxes = 0,
        IsLohotronReady = true,
    };
}