namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Bedge;

public class BedgeEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.Bedge;

    public static BedgeEvent Create()
    {
        return new BedgeEvent
        {
            Id = Guid.NewGuid(),
            Finished = true,
            EventDateTime = DateTime.Now,
        };
    }
}