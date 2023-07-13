using AntiClown.EntertainmentApi.Client.CommonEvents;
using AntiClown.EntertainmentApi.Client.DailyEvents;

namespace AntiClown.EntertainmentApi.Client;

public interface IAntiClownEntertainmentApiClient
{
    ICommonEventsClient CommonEvents { get; }
    IDailyEventsClient DailyEvents { get; }
}