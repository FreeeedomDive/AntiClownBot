namespace AntiClown.Api.Core.Economies.Domain.MassActions;

public class MassEconomyUpdate
{
    public MassScamCoinsUpdate? ScamCoins { get; set; }
    public int? LootBoxesDiff { get; set; }
    public DateTime? NextTribute { get; set; }
    public bool? IsLohotronReady { get; set; }
}