using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;

public class LotteryEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.Lottery;

    public Dictionary<Guid, LotterySlot[]> Results { get; set; } = new();

    public void AddParticipant(Guid userId)
    {
        if (Results.ContainsKey(userId))
        {
            return;
        }

        var possiblePrizes = Enum.GetValues<LotterySlot>();
        var userPrizes = Enumerable
            .Range(0, SlotsForParticipants)
            .Select(_ => possiblePrizes.SelectRandomItem())
            .ToArray();
        Results[userId] = userPrizes;
    }

    private const int SlotsForParticipants = 7;
}