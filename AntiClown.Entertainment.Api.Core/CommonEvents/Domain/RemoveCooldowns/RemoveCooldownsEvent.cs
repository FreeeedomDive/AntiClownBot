namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCooldowns;

public class RemoveCooldownsEvent : CommonEventBase
{
    public override CommonEventType Type => CommonEventType.RemoveCooldowns;
}