namespace AntiClown.Api.Dto.Economies;

public class UpdateScamCoinsDto
{
    public Guid UserId { get; set; }
    public int ScamCoinsDiff { get; set; }
    public string Reason { get; set; }
}