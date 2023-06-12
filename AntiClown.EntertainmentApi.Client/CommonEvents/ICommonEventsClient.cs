using AntiClown.EntertainmentApi.Client.CommonEvents.GuessNumber;
using AntiClown.EntertainmentApi.Client.CommonEvents.RemoveCoolDowns;

namespace AntiClown.EntertainmentApi.Client.CommonEvents;

public interface ICommonEventsClient
{
    IGuessNumberEventClient GuessNumber { get; }
    IRemoveCoolDownsEventClient RemoveCoolDowns { get; set; }
}