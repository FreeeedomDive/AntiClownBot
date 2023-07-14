using AntiClown.Entertainment.Api.Client.DailyEvents.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Client.DailyEvents.Announce;
using AntiClown.Entertainment.Api.Client.DailyEvents.PaymentsAndResets;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.DailyEvents;

public class DailyEventsClient : IDailyEventsClient
{
    public DailyEventsClient(RestClient restClient)
    {
        Announce = new AnnounceClient(restClient);
        PaymentsAndResets = new PaymentsAndResetsClient(restClient);
        ActiveDailyEventsIndex = new ActiveDailyEventsIndexClient(restClient);
    }

    public IAnnounceClient Announce { get; }
    public IPaymentsAndResetsClient PaymentsAndResets { get; }
    public IActiveDailyEventsIndexClient ActiveDailyEventsIndex { get; }
}