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

public interface IAntiClownEntertainmentApiClient
{
    IPartiesClient Parties { get; }
    IMinecraftAccountClient MinecraftAccount { get; }
    IMinecraftAuthClient MinecraftAuth { get; }
    IF1PredictionsClient F1Predictions { get; }
    IF1PredictionsStatsClient F1PredictionsStats { get; }
    IActiveDailyEventsIndexClient ActiveDailyEventsIndex { get; }
    IAnnounceEventClient AnnounceEvent { get; }
    IPaymentsAndResetsEventClient PaymentsAndResetsEvent { get; }
    IActiveEventsIndexClient ActiveEventsIndex { get; }
    IBedgeEventClient BedgeEvent { get; }
    IGuessNumberEventClient GuessNumberEvent { get; }
    ILotteryEventClient LotteryEvent { get; }
    IRaceDriversClient RaceDrivers { get; }
    IRaceEventClient RaceEvent { get; }
    IRaceTracksClient RaceTracks { get; }
    IRemoveCoolDownsEventClient RemoveCoolDownsEvent { get; }
    ITransfusionEventClient TransfusionEvent { get; }
}
