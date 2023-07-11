using AntiClown.EntertainmentApi.Client.CommonEvents.ActiveEventsIndex;
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
        Lottery = new LotteryClient(restClient);
        Race = new RaceClient(restClient);
        Transfusion = new TransfusionClient(restClient);
        ActiveEventsIndex = new ActiveEventsIndexClient(restClient);
    }

    public IGuessNumberEventClient GuessNumber { get; }
    public IRemoveCoolDownsEventClient RemoveCoolDowns { get; }
    public ILotteryClient Lottery { get; }
    public IRaceClient Race { get; }
    public ITransfusionClient Transfusion { get; }
    public IActiveEventsIndexClient ActiveEventsIndex { get; }
}