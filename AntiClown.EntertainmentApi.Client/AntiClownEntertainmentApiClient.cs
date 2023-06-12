using AntiClown.EntertainmentApi.Client.CommonEvents;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client;

public class AntiClownEntertainmentApiClient : IAntiClownEntertainmentApiClient
{
    public AntiClownEntertainmentApiClient(RestClient restClient)
    {
        CommonEvents = new CommonEventsClient(restClient);
    }

    public ICommonEventsClient CommonEvents { get; set; }
}