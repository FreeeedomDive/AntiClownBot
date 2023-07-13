using AntiClown.EntertainmentApi.Client.DailyEvents.Announce;

namespace AntiClown.EntertainmentApi.Client.DailyEvents;

public interface IDailyEventsClient
{
    IAnnounceClient Announce { get; }
}