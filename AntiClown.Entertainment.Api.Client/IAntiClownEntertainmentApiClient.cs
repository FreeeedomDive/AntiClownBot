using AntiClown.Entertainment.Api.Client.CommonEvents;
using AntiClown.Entertainment.Api.Client.DailyEvents;

namespace AntiClown.Entertainment.Api.Client;

public interface IAntiClownEntertainmentApiClient
{
    ICommonEventsClient CommonEvents { get; }
    IDailyEventsClient DailyEvents { get; }
}