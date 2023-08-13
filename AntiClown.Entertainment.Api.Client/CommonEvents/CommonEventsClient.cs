using AntiClown.Entertainment.Api.Client.CommonEvents.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Client.CommonEvents.Bedge;
using AntiClown.Entertainment.Api.Client.CommonEvents.GuessNumber;
using AntiClown.Entertainment.Api.Client.CommonEvents.Lottery;
using AntiClown.Entertainment.Api.Client.CommonEvents.Race;
using AntiClown.Entertainment.Api.Client.CommonEvents.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Client.CommonEvents.Transfusion;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents;

public class CommonEventsClient : ICommonEventsClient
{
    public CommonEventsClient(RestClient restClient)
    {
        GuessNumber = new GuessNumberEventClient(restClient);
        RemoveCoolDowns = new RemoveCoolDownsEventClient(restClient);
        Lottery = new LotteryEventClient(restClient);
        Race = new RaceEventClient(restClient);
        Transfusion = new TransfusionEventClient(restClient);
        Bedge = new BedgeEventClient(restClient);
        ActiveCommonEventsIndex = new ActiveCommonEventsIndexClient(restClient);
    }

    public IGuessNumberEventClient GuessNumber { get; }
    public IRemoveCoolDownsEventClient RemoveCoolDowns { get; }
    public ILotteryEventClient Lottery { get; }
    public IRaceEventClient Race { get; }
    public ITransfusionEventClient Transfusion { get; }
    public IBedgeEventClient Bedge { get; set; }
    public IActiveCommonEventsIndexClient ActiveCommonEventsIndex { get; }
}