namespace AntiClown.Api.Core.Economies.Domain.MassActions;

public class MassScamCoinsUpdate
{
    public int ScamCoinsDiff { get; set; }
    public string Reason { get; set; } = null!;
}