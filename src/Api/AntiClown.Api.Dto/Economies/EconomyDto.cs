namespace AntiClown.Api.Dto.Economies;

public class EconomyDto
{
    public Guid Id { get; set; }
    public int ScamCoins { get; set; }
    public DateTime NextTribute { get; set; }
    public int LootBoxes { get; set; }
    public bool IsLohotronReady { get; set; }
}