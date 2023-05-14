using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;

public class GuessNumberEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.GuessNumber;

    public Dictionary<Guid, int> Picks { get; set; } = new();
    public Dictionary<int, List<Guid>> NumberToUsers { get; set; } = new();
    public int Result { get; set; }

    public void AddPick(Guid userId, int newPick)
    {
        var hasPick = Picks.TryGetValue(userId, out var pick);
        if (hasPick)
        {
            NumberToUsers.Remove(pick, userId);
        }
        Picks[userId] = newPick;
        NumberToUsers.Add(newPick, userId);
    }
}