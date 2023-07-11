using AntiClown.EntertainmentApi.Client.CommonEvents.ActiveEventsIndex;
using AntiClown.EntertainmentApi.Client.CommonEvents.Bedge;
using AntiClown.EntertainmentApi.Client.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Client.CommonEvents.Lottery;
using AntiClown.EntertainmentApi.Client.CommonEvents.Race;
using AntiClown.EntertainmentApi.Client.CommonEvents.RemoveCoolDowns;
using AntiClown.EntertainmentApi.Client.CommonEvents.Transfusion;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents;

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
        ActiveEventsIndex = new ActiveEventsIndexClient(restClient);
    }

    public IGuessNumberEventClient GuessNumber { get; }
    public IRemoveCoolDownsEventClient RemoveCoolDowns { get; }
    public ILotteryEventClient Lottery { get; }
    public IRaceEventClient Race { get; }
    public ITransfusionEventClient Transfusion { get; }
    public IBedgeEventClient Bedge { get; set; }
    public IActiveEventsIndexClient ActiveEventsIndex { get; }
}