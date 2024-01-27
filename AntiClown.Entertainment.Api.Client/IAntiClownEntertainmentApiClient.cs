using AntiClown.Entertainment.Api.Client.CommonEvents;
using AntiClown.Entertainment.Api.Client.DailyEvents;
using AntiClown.Entertainment.Api.Client.F1Predictions;
using AntiClown.Entertainment.Api.Client.MinecraftAuth;
using AntiClown.Entertainment.Api.Client.Parties;

namespace AntiClown.Entertainment.Api.Client;

public interface IAntiClownEntertainmentApiClient
{
    ICommonEventsClient CommonEvents { get; }
    IDailyEventsClient DailyEvents { get; }
    IPartiesClient Parties { get; }
    IF1PredictionsClient F1Predictions { get; }
    IF1PredictionsStatsClient F1PredictionsStats { get; }
    IMinecraftRegisterClient MinecraftRegisterClient { get; }
}