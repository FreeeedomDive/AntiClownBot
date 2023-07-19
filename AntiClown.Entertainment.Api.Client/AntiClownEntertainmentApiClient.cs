using AntiClown.Entertainment.Api.Client.CommonEvents;
using AntiClown.Entertainment.Api.Client.DailyEvents;
using AntiClown.Entertainment.Api.Client.Parties;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client;

public class AntiClownEntertainmentApiClient : IAntiClownEntertainmentApiClient
{
    public AntiClownEntertainmentApiClient(RestClient restClient)
    {
        CommonEvents = new CommonEventsClient(restClient);
        DailyEvents = new DailyEventsClient(restClient);
        Parties = new PartiesClient(restClient);
    }

    public ICommonEventsClient CommonEvents { get; }
    public IDailyEventsClient DailyEvents { get; }
    public IPartiesClient Parties { get; }
}