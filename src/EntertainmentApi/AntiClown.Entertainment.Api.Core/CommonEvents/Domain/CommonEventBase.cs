namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

public abstract class CommonEventBase
{
    public Guid Id { get; set; }
    public abstract CommonEventType Type { get; }
    public DateTime EventDateTime { get; set; }
    public bool Finished { get; set; }
}