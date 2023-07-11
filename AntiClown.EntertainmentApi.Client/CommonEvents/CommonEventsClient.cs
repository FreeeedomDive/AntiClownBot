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
    }

    public IGuessNumberEventClient GuessNumber { get; }
    public IRemoveCoolDownsEventClient RemoveCoolDowns { get; set; }
    public ILotteryClient Lottery { get; set; }
    public IRaceClient Race { get; set; }
    public ITransfusionClient Transfusion { get; set; }
}