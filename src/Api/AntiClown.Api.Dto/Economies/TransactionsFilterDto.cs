namespace AntiClown.Api.Dto.Economies;

public class TransactionsFilterDto
{
    public Guid? UserId { get; set; }
    public DateTimeRangeDto? DateTimeRange { get; set; }
}