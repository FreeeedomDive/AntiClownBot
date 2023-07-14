using AntiClown.Entertainment.Api.Client.CommonEvents.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Client.CommonEvents.Bedge;
using AntiClown.Entertainment.Api.Client.CommonEvents.GuessNumber;
using AntiClown.Entertainment.Api.Client.CommonEvents.Lottery;
using AntiClown.Entertainment.Api.Client.CommonEvents.Race;
using AntiClown.Entertainment.Api.Client.CommonEvents.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Client.CommonEvents.Transfusion;

namespace AntiClown.Entertainment.Api.Client.CommonEvents;

public interface ICommonEventsClient
{
    IGuessNumberEventClient GuessNumber { get; }
    IRemoveCoolDownsEventClient RemoveCoolDowns { get; }
    ILotteryEventClient Lottery { get; }
    IRaceEventClient Race { get; }
    ITransfusionEventClient Transfusion { get; }
    IBedgeEventClient Bedge { get; set; }
    IActiveCommonEventsIndexClient ActiveCommonEventsIndex { get; }
}