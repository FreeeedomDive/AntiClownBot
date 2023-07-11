using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;

public class LotteryEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.Lottery;

    public Dictionary<Guid, LotteryParticipant> Participants { get; set; } = new();

    public void AddParticipant(Guid userId)
    {
        if (Participants.ContainsKey(userId))
        {
            return;
        }

        var possiblePrizes = Enum.GetValues<LotterySlot>();
        var userPrizes = Enumerable
            .Range(0, SlotsForParticipant)
            .Select(_ => possiblePrizes.SelectRandomItem())
            .ToArray();
        Participants[userId] = new LotteryParticipant
        {
            UserId = userId,
            Slots = userPrizes,
            Prize = LotteryPrizeCalculator.Calculate(userPrizes),
        };
    }

    public static LotteryEvent Create()
    {
        return new LotteryEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.UtcNow,
            Participants = new Dictionary<Guid, LotteryParticipant>(),
        };
    }

    private const int SlotsForParticipant = 7;
}