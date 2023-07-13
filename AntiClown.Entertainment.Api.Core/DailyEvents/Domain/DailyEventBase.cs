namespace AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

public abstract class DailyEventBase
{
    public abstract DailyEventType Type { get; }
}