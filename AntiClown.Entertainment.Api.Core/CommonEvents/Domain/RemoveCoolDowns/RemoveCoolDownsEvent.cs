namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCoolDowns;

public class RemoveCoolDownsEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.RemoveCoolDowns;

    public static RemoveCoolDownsEvent Create()
    {
        return new RemoveCoolDownsEvent
        {
            Id = Guid.NewGuid(),
            Finished = true,
            EventDateTime = DateTime.UtcNow,
        };
    }
}