using System.Text;
using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Models.Race
{
    public enum SectorType
    {
        Cornering,
        Acceleration,
        Breaking
    }

    public class RaceModel
    {
        public RaceModel(
            IDiscordClientWrapper discordClientWrapper,
            IRandomizer randomizer,
            IGuildSettingsService guildSettingsService,
            IApiClient apiClient
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.randomizer = randomizer;
            this.guildSettingsService = guildSettingsService;
            this.apiClient = apiClient;
        }

        public async Task<RaceModel> CreateAsync()
        {
            currentTrack = SelectTrack();
            FillSectors();

            var driversContent = await File.ReadAllTextAsync("../Files/StatisticsFiles/drivers.json");
            var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
            if (driversModels is null)
            {
                throw new ArgumentException("А где челики?");
            }

            var pos = 1;
            drivers = driversModels.Select(model => new Driver
            {
                DiscordId = 0,
                IsUser = false,
                DriverModel = model,
                //UsableEmoji = Utility.Emoji($":{model.ShortName}:"),
                IsFinished = false,
                BestLap = -1,
                TotalTime = 0,
                TimesPerSector = new int[TotalLaps * TotalSectorsPerLap]
            }).Shuffle(randomizer).ForEach(d =>
            {
                var i = pos++;
                d.StartPosition = i;
            }).ToList();

            mainRaceMessage = await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, await GetStartingGrid());
            isJoinable = true;
            JoinRace(await discordClientWrapper.Members.GetBotIdAsync());

            return this;
        }

        public async void JoinRace(ulong userId)
        {
            if (!isJoinable) return;

            if (drivers.Any(d => d.DiscordId == userId)) return;

            var driver = drivers.Where(d => !d.IsUser).SelectRandomItem(randomizer);
            var member = await discordClientWrapper.Members.GetAsync(userId);
            driver.DiscordId = userId;
            driver.Username = member.ServerOrUserName();
            driver.IsUser = true;

            var newStartingGrid = await GetStartingGrid();
            await discordClientWrapper.Messages.ModifyAsync(mainRaceMessage, newStartingGrid);
        }

        private TrackModel SelectTrack()
        {
            var tracksContent = File.ReadAllText("../Files/StatisticsFiles/tracks.json");
            var tracks = JsonConvert.DeserializeObject<IEnumerable<TrackModel>>(tracksContent);
            if (tracks is null)
            {
                throw new ArgumentException("А где треки?");
            }

            return tracks.SelectRandomItem(randomizer);
        }

        private void FillSectors()
        {
            var totalFilled = 0;
            for (var i = 0; i < currentTrack.CorneringDifficulty; i++)
            {
                sectors[i] = SectorType.Cornering;
            }

            totalFilled += currentTrack.CorneringDifficulty;

            for (var i = 0; i < currentTrack.AccelerationDifficulty; i++)
            {
                sectors[totalFilled + i] = SectorType.Acceleration;
            }

            totalFilled += currentTrack.AccelerationDifficulty;

            for (var i = 0; i < currentTrack.BreakingDifficulty; i++)
            {
                sectors[totalFilled + i] = SectorType.Breaking;
            }

            sectors = sectors.Shuffle(randomizer).ToArray();
        }

        public async Task<string> GetStartingGrid()
        {
            var tasks = drivers.Select(async (d, i) =>
            {
                var pos = i + 1;
                //var emoji = d.UsableEmoji;
                var result = (pos < 10 ? " " : "") + $"{pos}.\t{d.DriverModel.ShortName}";
                if (!d.IsUser) return result;

                var member = await discordClientWrapper.Members.GetAsync(d.DiscordId);
                result += $"\t - {member.ServerOrUserName()}";

                return result;
            });

            var driversString = await Task.WhenAll(tasks);

            return $"Трасса {currentTrack.Name}\n```\n{string.Join("\n", driversString)}\n```";
        }

        public void StartRace()
        {
            isJoinable = false;
            foreach (var driver in drivers)
            {
                new Thread(() => StartRaceForDriver(driver))
                {
                    IsBackground = true
                }.Start();
            }

            var updatingThread = new Thread(async () =>
            {
                while (!isFinished)
                {
                    await Task.Delay(3 * 1000);
                    if (isFinished) return;
                    var snapShot = MakeSnapshot();
                    await discordClientWrapper.Messages.ModifyAsync(mainRaceMessage, snapShot);
                }
            })
            {
                IsBackground = true
            };
            updatingThread.Start();
        }

        private async void StartRaceForDriver(Driver driver)
        {
            var idealSectorTime = currentTrack.IdealTime / TotalSectorsPerLap;
            var maxCorneringSectorTime = (int)(idealSectorTime / Math.Pow(driver.DriverModel.CorneringStat, 2d / 5));
            var maxAccelerationSectorTime = (int)(idealSectorTime / Math.Pow(driver.DriverModel.AccelerationStat, 2d / 5));
            var maxBreakingSectorTime = (int)(idealSectorTime / Math.Pow(driver.DriverModel.BreakingStat, 2d / 5));
            for (var i = 0; i < TotalLaps * TotalSectorsPerLap; i++)
            {
                var sectorNumberOnLap = i % TotalSectorsPerLap;
                if (sectorNumberOnLap == 0)
                {
                    driver.CurrentLapTime = 0;
                    driver.CurrentLap++;
                    currentLap = Math.Max(driver.CurrentLap, currentLap);
                }

                var startPositionPenalty = i == 0 ? (driver.StartPosition - 1) * 85 : 0;
                var sectorTime = sectors[sectorNumberOnLap] switch
                {
                    SectorType.Cornering => randomizer.GetRandomNumberBetween(idealSectorTime, maxCorneringSectorTime),
                    SectorType.Acceleration => randomizer.GetRandomNumberBetween(idealSectorTime, maxAccelerationSectorTime),
                    SectorType.Breaking => randomizer.GetRandomNumberBetween(idealSectorTime, maxBreakingSectorTime),
                    _ => idealSectorTime
                };
                sectorTime += startPositionPenalty;

                await Task.Delay(sectorTime);

                driver.TotalSectorsPassed++;
                driver.TotalTime += sectorTime;
                driver.CurrentLapTime += sectorTime;
                driver.TimesPerSector[i] = driver.TotalTime;

                lock (orderingLocker)
                {
                    drivers = drivers.OrderByDescending(d =>
                        d.TotalSectorsPassed == 0
                            ? 0
                            : d.TotalSectorsPassed * 10000000000 - d.TimesPerSector[d.TotalSectorsPassed - 1]).ToList();
                }

                if (sectorNumberOnLap == TotalSectorsPerLap - 1)
                {
                    driver.BestLap = Math.Min(driver.BestLap, driver.CurrentLapTime);
                    lock (bestLapLocker)
                    {
                        bestLap = Math.Min(bestLap, driver.CurrentLapTime);
                        if (bestLap == driver.CurrentLapTime)
                            bestLapHolder = driver.DriverModel.Name;
                    }
                }
            }

            driver.IsFinished = true;
            if (drivers.All(d => d.IsFinished))
            {
                await MakeResult();
            }
        }

        private string MakeSnapshot()
        {
            var sb = new StringBuilder($"Трасса {currentTrack.Name}\nТекущий круг: {currentLap} / {TotalLaps}\n```");

            var driversInfo = drivers.Select((driver, i) =>
            {
                var pos = i + 1;
                //var ebalo = driver.UsableEmoji;

                var lap = driver.CurrentLap;

                var result = (pos < 10 ? " " : "") + $"{pos}.\t{driver.DriverModel.ShortName}";

                if (lap != 0 && driver.TotalSectorsPassed != 0)
                {
                    var gapToLeader = Math.Abs(driver.TimesPerSector[driver.TotalSectorsPassed - 1] -
                                               drivers[0].TimesPerSector[driver.TotalSectorsPassed - 1]);

                    result += $"\tКРУГ {lap}\t+{Utility.NormalizeTime(gapToLeader)}";
                }

                if (driver.IsUser) result += $"\t - {driver.Username}";

                return result;
            });

            sb.Append(string.Join("\n", driversInfo));

            if (currentLap > 1)
            {
                sb.Append('\n')
                    .Append($"ЛУЧШИЙ КРУГ: {Utility.NormalizeTime(bestLap)} от {bestLapHolder}");
            }

            return sb.Append("```").ToString();
        }

        private async Task MakeResult()
        {
            isFinished = true;

            var botPosition = drivers.Count;
            var botId = await discordClientWrapper.Members.GetBotIdAsync();

            var sb = new StringBuilder($"РЕЗУЛЬТАТЫ ГОНОЧКИ В {currentTrack.Name}\n```");
            var driversInfo = drivers.Select(async (d, i) =>
            {
                var pos = i + 1;
                if (d.DiscordId == botId)
                {
                    botPosition = pos;
                }

                var result = (pos < 10 ? " " : "") + $"{pos}.\t{d.DriverModel.ShortName}";
                if (!d.IsUser) return result;

                var user = await discordClientWrapper.Members.GetAsync(d.DiscordId);
                result += $"\t{user.ServerOrUserName()}";

                if (pos < botPosition && pos <= 10)
                {
                    var pts = Points[i];
                    await apiClient.Users.ChangeUserRatingAsync(d.DiscordId, pts, $"{pos} место в гонке");
                    result += $"\t+{pts} scam coins";
                }

                // улучшаются только те гонщики, которые проиграли позиции относительно старта
                var diff = pos - d.StartPosition;
                for (var j = 0; j < diff; j++)
                {
                    switch (randomizer.GetRandomNumberBetween(0, 3))
                    {
                        case 0:
                            d.DriverModel.CorneringStat += 0.001f;
                            break;
                        case 1:
                            d.DriverModel.AccelerationStat += 0.001f;
                            break;
                        case 2:
                            d.DriverModel.BreakingStat += 0.001f;
                            break;
                    }
                }

                return result;
            });

            sb.Append(string.Join("\n", driversInfo)).Append("```");
            await discordClientWrapper.Messages.ModifyAsync(mainRaceMessage, sb.ToString());

            var models = drivers.Select(d => d.DriverModel).ToList();
            await File.WriteAllTextAsync("../Files/StatisticsFiles/drivers.json", JsonConvert.SerializeObject(models, Formatting.Indented));

            // TODO вынести в эвент 
            OnRaceEnd();
        }

        public static readonly int[] Points = new[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 }.Select(x => x * 100).ToArray();

        public ulong JoinableMessageId { get; init; }
        public Action OnRaceEnd { get; init; }

        private bool isJoinable;

        private readonly object bestLapLocker = new();
        private readonly object orderingLocker = new();

        private List<Driver> drivers;
        private TrackModel currentTrack;
        private bool isFinished;

        private const int TotalLaps = 5;
        private const int TotalSectorsPerLap = 50;

        private int currentLap;
        private int bestLap = 999999;
        private string bestLapHolder;

        private SectorType[] sectors = new SectorType[TotalSectorsPerLap];

        private DiscordMessage mainRaceMessage;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IRandomizer randomizer;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IApiClient apiClient;
    }
}