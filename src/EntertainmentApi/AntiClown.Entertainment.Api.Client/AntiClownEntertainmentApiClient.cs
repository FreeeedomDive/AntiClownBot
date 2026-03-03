/* Generated file */

using AntiClown.Entertainment.Api.Client.Parties;
using AntiClown.Entertainment.Api.Client.MinecraftAccount;
using AntiClown.Entertainment.Api.Client.MinecraftAuth;
using AntiClown.Entertainment.Api.Client.F1Bingo;
using AntiClown.Entertainment.Api.Client.F1ChampionshipPredictions;
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
    public AntiClownEntertainmentApiClient(RestSharp.RestClient client)
    {
        Parties = new PartiesClient(client);
        MinecraftAccount = new MinecraftAccountClient(client);
        MinecraftAuth = new MinecraftAuthClient(client);
        F1Bingo = new F1BingoClient(client);
        F1ChampionshipPredictions = new F1ChampionshipPredictionsClient(client);
        F1Predictions = new F1PredictionsClient(client);
        F1PredictionsStats = new F1PredictionsStatsClient(client);
        ActiveDailyEventsIndex = new ActiveDailyEventsIndexClient(client);
        AnnounceEvent = new AnnounceEventClient(client);
        PaymentsAndResetsEvent = new PaymentsAndResetsEventClient(client);
        ActiveEventsIndex = new ActiveEventsIndexClient(client);
        BedgeEvent = new BedgeEventClient(client);
        GuessNumberEvent = new GuessNumberEventClient(client);
        LotteryEvent = new LotteryEventClient(client);
        RaceDrivers = new RaceDriversClient(client);
        RaceEvent = new RaceEventClient(client);
        RaceTracks = new RaceTracksClient(client);
        RemoveCoolDownsEvent = new RemoveCoolDownsEventClient(client);
        TransfusionEvent = new TransfusionEventClient(client);
    }

    public IPartiesClient Parties { get; }
    public IMinecraftAccountClient MinecraftAccount { get; }
    public IMinecraftAuthClient MinecraftAuth { get; }
    public IF1BingoClient F1Bingo { get; }
    public IF1ChampionshipPredictionsClient F1ChampionshipPredictions { get; }
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
