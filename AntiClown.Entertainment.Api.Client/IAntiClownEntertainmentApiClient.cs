using AntiClown.Entertainment.Api.Client.CommonEvents;
using AntiClown.Entertainment.Api.Client.DailyEvents;
using AntiClown.Entertainment.Api.Client.Parties;

namespace AntiClown.Entertainment.Api.Client;

public interface IAntiClownEntertainmentApiClient
{
    ICommonEventsClient CommonEvents { get; }
    IDailyEventsClient DailyEvents { get; }
    public IPartiesClient Parties { get; }
}