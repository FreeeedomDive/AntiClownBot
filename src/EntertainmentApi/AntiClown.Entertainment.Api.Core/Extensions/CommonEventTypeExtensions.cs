using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.Extensions;

public static class CommonEventTypeExtensions
{
    public static bool IsNightTimeEvent(this CommonEventType commonEventType)
    {
        return commonEventType switch
        {
            CommonEventType.GuessNumber => false,
            CommonEventType.Lottery => false,
            CommonEventType.Race => false,
            CommonEventType.RemoveCoolDowns => false,
            CommonEventType.Transfusion => false,
            CommonEventType.Bedge => true,
            _ => throw new ArgumentOutOfRangeException(nameof(commonEventType), commonEventType, null),
        };
    }
}