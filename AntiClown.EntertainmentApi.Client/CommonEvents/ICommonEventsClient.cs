using AntiClown.EntertainmentApi.Client.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Client.CommonEvents.Lottery;
using AntiClown.EntertainmentApi.Client.CommonEvents.Race;
using AntiClown.EntertainmentApi.Client.CommonEvents.RemoveCoolDowns;
using AntiClown.EntertainmentApi.Client.CommonEvents.Transfusion;

namespace AntiClown.EntertainmentApi.Client.CommonEvents;

public interface ICommonEventsClient
{
    IGuessNumberEventClient GuessNumber { get; }
    IRemoveCoolDownsEventClient RemoveCoolDowns { get; set; }
    ILotteryClient Lottery { get; set; }
    IRaceClient Race { get; set; }
    ITransfusionClient Transfusion { get; set; }
}