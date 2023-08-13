namespace AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;

public class LotteryParticipantDto
{
    public Guid UserId { get; set; }
    public LotterySlotDto[] Slots { get; set; }
    public int Prize { get; set; }
}