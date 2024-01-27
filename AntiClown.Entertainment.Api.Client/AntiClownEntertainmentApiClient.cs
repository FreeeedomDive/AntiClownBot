using AntiClown.Entertainment.Api.Client.CommonEvents;
using AntiClown.Entertainment.Api.Client.DailyEvents;
using AntiClown.Entertainment.Api.Client.F1Predictions;
using AntiClown.Entertainment.Api.Client.MinecraftAuth;
using AntiClown.Entertainment.Api.Client.Parties;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client;

public class AntiClownEntertainmentApiClient : IAntiClownEntertainmentApiClient
{
    public AntiClownEntertainmentApiClient(RestClient restClient)
    {
        CommonEvents = new CommonEventsClient(restClient);
        DailyEvents = new DailyEventsClient(restClient);
        Parties = new PartiesClient(restClient);
        F1Predictions = new F1PredictionsClient(restClient);
        F1PredictionsStats = new F1PredictionsStatsClient(restClient);
        MinecraftRegisterClient = new MinecraftRegisterClient(restClient);
    }

    public ICommonEventsClient CommonEvents { get; }
    public IDailyEventsClient DailyEvents { get; }
    public IPartiesClient Parties { get; }
    public IF1PredictionsClient F1Predictions { get; }
    public IF1PredictionsStatsClient F1PredictionsStats { get; }
    public IMinecraftRegisterClient MinecraftRegisterClient { get; }
}