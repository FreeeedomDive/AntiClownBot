namespace AntiClown.Api.Dto.Economies;

public class UpdateLootBoxesDto
{
    public Guid UserId { get; set; }
    public int LootBoxesDiff { get; set; }
}