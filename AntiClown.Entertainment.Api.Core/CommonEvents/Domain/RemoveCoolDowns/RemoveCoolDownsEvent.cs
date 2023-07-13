namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCoolDowns;

public class RemoveCoolDownsEvent : CommonEventBase
{
    public static RemoveCoolDownsEvent Create()
    {
        return new RemoveCoolDownsEvent
        {
            Id = Guid.NewGuid(),
            Finished = true,
            EventDateTime = DateTime.UtcNow,
        };
    }

    public override CommonEventType Type => CommonEventType.RemoveCoolDowns;
}