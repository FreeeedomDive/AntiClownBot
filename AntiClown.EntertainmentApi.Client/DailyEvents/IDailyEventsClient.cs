using AntiClown.EntertainmentApi.Client.DailyEvents.Announce;
using AntiClown.EntertainmentApi.Client.DailyEvents.PaymentsAndResets;

namespace AntiClown.EntertainmentApi.Client.DailyEvents;

public interface IDailyEventsClient
{
    IAnnounceClient Announce { get; }
    IPaymentsAndResetsClient PaymentsAndResets { get; }
}