using AntiClown.EntertainmentApi.Client.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Client.CommonEvents.RemoveCoolDowns;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents;

public class CommonEventsClient : ICommonEventsClient
{
    public CommonEventsClient(RestClient restClient)
    {
        GuessNumber = new GuessNumberEventClient(restClient);
        RemoveCoolDowns = new RemoveCoolDownsEventClient(restClient);
    }

    public IGuessNumberEventClient GuessNumber { get; }
    public IRemoveCoolDownsEventClient RemoveCoolDowns { get; set; }
}