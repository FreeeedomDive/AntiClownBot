namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Bedge;

public class BedgeEvent : CommonEventBase
{
    public static BedgeEvent Create()
    {
        return new BedgeEvent
        {
            Id = Guid.NewGuid(),
            Finished = true,
            EventDateTime = DateTime.UtcNow,
        };
    }

    public override CommonEventType Type => CommonEventType.Bedge;
}