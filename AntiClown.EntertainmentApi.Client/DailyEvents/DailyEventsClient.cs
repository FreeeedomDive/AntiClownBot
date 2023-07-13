using AntiClown.EntertainmentApi.Client.DailyEvents.Announce;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.DailyEvents;

public class DailyEventsClient : IDailyEventsClient
{
    public DailyEventsClient(RestClient restClient)
    {
        Announce = new AnnounceClient(restClient);
    }

    public IAnnounceClient Announce { get; }
}