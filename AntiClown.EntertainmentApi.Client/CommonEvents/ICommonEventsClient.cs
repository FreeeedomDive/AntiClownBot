using AntiClown.EntertainmentApi.Client.CommonEvents.ActiveEventsIndex;
using AntiClown.EntertainmentApi.Client.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Client.CommonEvents.Lottery;
using AntiClown.EntertainmentApi.Client.CommonEvents.Race;
using AntiClown.EntertainmentApi.Client.CommonEvents.RemoveCoolDowns;
using AntiClown.EntertainmentApi.Client.CommonEvents.Transfusion;

namespace AntiClown.EntertainmentApi.Client.CommonEvents;

public interface ICommonEventsClient
{
    IGuessNumberEventClient GuessNumber { get; }
    IRemoveCoolDownsEventClient RemoveCoolDowns { get; }
    ILotteryClient Lottery { get; }
    IRaceClient Race { get; }
    ITransfusionClient Transfusion { get;  }
    IActiveEventsIndexClient ActiveEventsIndex { get; }
}