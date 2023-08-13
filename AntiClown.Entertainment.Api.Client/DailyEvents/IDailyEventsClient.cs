using AntiClown.Entertainment.Api.Client.DailyEvents.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Client.DailyEvents.Announce;
using AntiClown.Entertainment.Api.Client.DailyEvents.PaymentsAndResets;

namespace AntiClown.Entertainment.Api.Client.DailyEvents;

public interface IDailyEventsClient
{
    IAnnounceClient Announce { get; }
    IPaymentsAndResetsClient PaymentsAndResets { get; }
    IActiveDailyEventsIndexClient ActiveDailyEventsIndex { get; }
}