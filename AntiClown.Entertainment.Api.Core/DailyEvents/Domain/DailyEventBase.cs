namespace AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

public abstract class DailyEventBase
{
    public Guid Id { get; set; }
    public DateTime EventDateTime { get; set; }
    public abstract DailyEventType Type { get; }
}