using System;
using System.Linq;

namespace AntiClownBot.Events.DailyEvents;

public class RemoveOldPartiesSystemEvent: BaseEvent
{
    public override void ExecuteAsync()
    {
        var partiesToRemove = Config
            .OpenParties
            .Where(kv => (DateTime.Now - kv.Value.CreationDate).Days > 0)
            .Select(kv => kv.Key);

        foreach (var partyId in partiesToRemove)
        {
            Config.OpenParties.Remove(partyId);
        }

        Config.Save();
    }

    protected override string BackStory() => string.Empty;
}