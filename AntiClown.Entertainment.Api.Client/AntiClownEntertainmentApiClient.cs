/* Generated file */

using AntiClown.Entertainment.Api.Client.Parties;
using AntiClown.Entertainment.Api.Client.MinecraftAccount;
using AntiClown.Entertainment.Api.Client.MinecraftAuth;
using AntiClown.Entertainment.Api.Client.F1Predictions;
using AntiClown.Entertainment.Api.Client.F1PredictionsStats;
using AntiClown.Entertainment.Api.Client.ActiveDailyEventsIndex;
using AntiClown.Entertainment.Api.Client.AnnounceEvent;
using AntiClown.Entertainment.Api.Client.PaymentsAndResetsEvent;
using AntiClown.Entertainment.Api.Client.ActiveEventsIndex;
using AntiClown.Entertainment.Api.Client.BedgeEvent;
using AntiClown.Entertainment.Api.Client.GuessNumberEvent;
using AntiClown.Entertainment.Api.Client.LotteryEvent;
using AntiClown.Entertainment.Api.Client.RaceDrivers;
using AntiClown.Entertainment.Api.Client.RaceEvent;
using AntiClown.Entertainment.Api.Client.RaceTracks;
using AntiClown.Entertainment.Api.Client.RemoveCoolDownsEvent;
using AntiClown.Entertainment.Api.Client.TransfusionEvent;

namespace AntiClown.Entertainment.Api.Client;

public class AntiClownEntertainmentApiClient : IAntiClownEntertainmentApiClient
{
    public AntiClownEntertainmentApiClient(RestSharp.RestClient restClient)
    {
        Parties = new PartiesClient(restClient);
        MinecraftAccount = new MinecraftAccountClient(restClient);
        MinecraftAuth = new MinecraftAuthClient(restClient);
        F1Predictions = new F1PredictionsClient(restClient);
        F1PredictionsStats = new F1PredictionsStatsClient(restClient);
        ActiveDailyEventsIndex = new ActiveDailyEventsIndexClient(restClient);
        AnnounceEvent = new AnnounceEventClient(restClient);
        PaymentsAndResetsEvent = new PaymentsAndResetsEventClient(restClient);
        ActiveEventsIndex = new ActiveEventsIndexClient(restClient);
        BedgeEvent = new BedgeEventClient(restClient);
        GuessNumberEvent = new GuessNumberEventClient(restClient);
        LotteryEvent = new LotteryEventClient(restClient);
        RaceDrivers = new RaceDriversClient(restClient);
        RaceEvent = new RaceEventClient(restClient);
        RaceTracks = new RaceTracksClient(restClient);
        RemoveCoolDownsEvent = new RemoveCoolDownsEventClient(restClient);
        TransfusionEvent = new TransfusionEventClient(restClient);
    }

    public IPartiesClient Parties { get; }
    public IMinecraftAccountClient MinecraftAccount { get; }
    public IMinecraftAuthClient MinecraftAuth { get; }
    public IF1PredictionsClient F1Predictions { get; }
    public IF1PredictionsStatsClient F1PredictionsStats { get; }
    public IActiveDailyEventsIndexClient ActiveDailyEventsIndex { get; }
    public IAnnounceEventClient AnnounceEvent { get; }
    public IPaymentsAndResetsEventClient PaymentsAndResetsEvent { get; }
    public IActiveEventsIndexClient ActiveEventsIndex { get; }
    public IBedgeEventClient BedgeEvent { get; }
    public IGuessNumberEventClient GuessNumberEvent { get; }
    public ILotteryEventClient LotteryEvent { get; }
    public IRaceDriversClient RaceDrivers { get; }
    public IRaceEventClient RaceEvent { get; }
    public IRaceTracksClient RaceTracks { get; }
    public IRemoveCoolDownsEventClient RemoveCoolDownsEvent { get; }
    public ITransfusionEventClient TransfusionEvent { get; }
}
