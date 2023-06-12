using AntiClown.EntertainmentApi.Client.CommonEvents;

namespace AntiClown.EntertainmentApi.Client;

public interface IAntiClownEntertainmentApiClient
{
    ICommonEventsClient CommonEvents { get; set; }
}