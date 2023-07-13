namespace AntiClown.Entertainment.Api.Core.DailyEvents.Domain.Announce;

public class AnnounceEvent : DailyEventBase
{
    public override DailyEventType Type => DailyEventType.Announce;
    
    public Dictionary<Guid, int> Earnings { get; set; }
}