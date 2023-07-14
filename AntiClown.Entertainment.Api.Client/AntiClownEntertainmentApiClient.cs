using AntiClown.Entertainment.Api.Client.CommonEvents;
using AntiClown.Entertainment.Api.Client.DailyEvents;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client;

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