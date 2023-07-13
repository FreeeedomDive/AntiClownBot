using AntiClown.EntertainmentApi.Client.CommonEvents;
using AntiClown.EntertainmentApi.Client.DailyEvents;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client;

public class AntiClownEntertainmentApiClient : IAntiClownEntertainmentApiClient
{
    public AntiClownEntertainmentApiClient(RestClient restClient)
    {
        CommonEvents = new CommonEventsClient(restClient);
        DailyEvents = new DailyEventsClient(restClient);
    }

    public ICommonEventsClient CommonEvents { get; set; }
    public IDailyEventsClient DailyEvents { get; }
}