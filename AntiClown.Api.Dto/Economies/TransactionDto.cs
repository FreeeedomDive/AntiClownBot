namespace AntiClown.Api.Dto.Economies;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int ScamCoinDiff { get; set; }
    public DateTime DateTime { get; set; }
    public string Reason { get; set; }
}