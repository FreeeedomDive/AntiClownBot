﻿using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;

public class GuessNumberEvent : CommonEventBase
{
    public void AddPick(Guid userId, GuessNumberPick newPick)
    {
        var hasPick = Picks.TryGetValue(userId, out var pick);
        if (hasPick)
        {
            NumberToUsers.Remove(pick, userId);
        }

        Picks[userId] = newPick;
        NumberToUsers.Add(newPick, userId);
    }

    public static GuessNumberEvent Create()
    {
        return new GuessNumberEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.UtcNow,
            Picks = new Dictionary<Guid, GuessNumberPick>(),
            NumberToUsers = new Dictionary<GuessNumberPick, List<Guid>>(),
            Result = Enum.GetValues<GuessNumberPick>().SelectRandomItem(),
        };
    }

    public override CommonEventType Type => CommonEventType.GuessNumber;

    public Dictionary<Guid, GuessNumberPick> Picks { get; set; } = new();
    public Dictionary<GuessNumberPick, List<Guid>> NumberToUsers { get; set; } = new();
    public GuessNumberPick Result { get; set; }
}