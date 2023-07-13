using AntiClown.EntertainmentApi.Client.CommonEvents.ActiveEventsIndex;
using AntiClown.EntertainmentApi.Client.CommonEvents.Bedge;
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
    ILotteryEventClient Lottery { get; }
    IRaceEventClient Race { get; }
    ITransfusionEventClient Transfusion { get;  }
    IBedgeEventClient Bedge { get; set; }
    IActiveCommonEventsIndexClient ActiveCommonEventsIndex { get; }
}