namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;

public class LotteryParticipant
{
    public Guid UserId { get; set; }
    public LotterySlot[] Slots { get; set; }
    public int Prize { get; set; }
}