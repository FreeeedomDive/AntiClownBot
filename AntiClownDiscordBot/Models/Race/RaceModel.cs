using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBot.Helpers;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace AntiClownBot.Models.Race
{
    public enum SectorType
    {
        Cornering,
        Acceleration,
        Breaking
    }

    public class RaceModel
    {
        private bool _isJoinable;

        private object _bestLapLocker = new();
        private object _orderingLocker = new();

        private Configuration _config;
        private List<Driver> _drivers;
        private TrackModel _currentTrack;
        private bool _isFinished;

        private const int TotalLaps = 5;
        private const int TotalSectorsPerLap = 50;

        private int _currentLap;
        private int _bestLap = 999999;
        private string _bestLapHolder;

        private SectorType[] _sectors = new SectorType[TotalSectorsPerLap];

        private DiscordMessage _mainRaceMessage;
        public ulong JoinableMessageId;

        public static readonly int[] Points = new[] {25, 18, 15, 12, 10, 8, 6, 4, 2, 1}.Select(x => x * 100).ToArray();

        public RaceModel()
        {
            _config = Configuration.GetConfiguration();

            _currentTrack = SelectTrack();
            FillSectors();

            var driversContent = File.ReadAllText("drivers.json");
            var driversModels = JsonConvert.DeserializeObject<IEnumerable<DriverModel>>(driversContent);
            if (driversModels is null) throw new ArgumentException("А где челики?");
            var pos = 1;
            _drivers = driversModels.Select(model => new Driver
            {
                DiscordId = 0,
                IsUser = false,
                DriverModel = model,
                UsableEmoji = Utility.Emoji($":{model.ShortName}:"),
                IsFinished = false,
                BestLap = -1,
                TotalTime = 0,
                TimesPerSector = new int[TotalLaps * TotalSectorsPerLap]
            }).Shuffle().ForEach(d =>
            {
                var i = pos++;
                d.StartPosition = i;
            }).ToList();

            SetupMainMessage();
        }

        private async void SetupMainMessage()
        {
            _mainRaceMessage = await Utility.SendMessageToBotChannel(GetStartingGrid());
            _isJoinable = true;
            JoinRace(Constants.BotId);
        }

        public async void JoinRace(ulong userId)
        {
            if (!_isJoinable) return;

            if (_drivers.Any(d => d.DiscordId == userId)) return;

            var driver = _drivers.Where(d => !d.IsUser).SelectRandomItem();
            var member = Configuration.GetServerMember(userId).ServerOrUserName();
            driver.DiscordId = userId;
            driver.Username = member;
            driver.IsUser = true;

            await _mainRaceMessage.ModifyAsync(GetStartingGrid());
        }

        private static TrackModel SelectTrack()
        {
            var tracksContent = File.ReadAllText("tracks.json");
            var tracks = JsonConvert.DeserializeObject<IEnumerable<TrackModel>>(tracksContent);
            if (tracks is null) throw new ArgumentException("А где треки?");

            return tracks.SelectRandomItem();
        }

        private void FillSectors()
        {
            var totalFilled = 0;
            for (var i = 0; i < _currentTrack.CorneringDifficulty; i++)
            {
                _sectors[i] = SectorType.Cornering;
            }

            totalFilled += _currentTrack.CorneringDifficulty;

            for (var i = 0; i < _currentTrack.AccelerationDifficulty; i++)
            {
                _sectors[totalFilled + i] = SectorType.Acceleration;
            }

            totalFilled += _currentTrack.AccelerationDifficulty;

            for (var i = 0; i < _currentTrack.BreakingDifficulty; i++)
            {
                _sectors[totalFilled + i] = SectorType.Breaking;
            }

            _sectors = _sectors.Shuffle().ToArray();
        }

        public string GetStartingGrid()
        {
            var driversString = _drivers.Select((d, i) =>
            {
                var pos = i + 1;
                var emoji = d.UsableEmoji;
                var result = $"{pos}.\t{emoji}";
                if (!d.IsUser) return result;

                var member = Configuration.GetServerMember(d.DiscordId);
                result += $"\t - {member.ServerOrUserName()}";

                return result;
            });

            return $"Трасса {_currentTrack.Name}\n{string.Join("\n", driversString)}";
        }

        public void StartRace()
        {
            _isJoinable = false;
            foreach (var driver in _drivers)
            {
                new Thread(() => StartRaceForDriver(driver))
                {
                    IsBackground = true
                }.Start();
            }

            var updatingThread = new Thread(async () =>
            {
                while (!_isFinished)
                {
                    await Task.Delay(3 * 1000);
                    if (_isFinished) return;
                    var snapShot = MakeSnapshot();
                    await _mainRaceMessage.ModifyAsync(snapShot);
                }
            })
            {
                IsBackground = true
            };
            updatingThread.Start();
        }

        private async void StartRaceForDriver(Driver driver)
        {
            var idealSectorTime = _currentTrack.IdealTime / TotalSectorsPerLap;
            var maxCorneringSectorTime = (int) (idealSectorTime / Math.Pow(driver.DriverModel.CorneringStat, 2d / 5));
            var maxAccelerationSectorTime =
                (int) (idealSectorTime / Math.Pow(driver.DriverModel.AccelerationStat, 2d / 5));
            var maxBreakingSectorTime = (int) (idealSectorTime / Math.Pow(driver.DriverModel.BreakingStat, 2d / 5));
            for (var i = 0; i < TotalLaps * TotalSectorsPerLap; i++)
            {
                var sectorNumberOnLap = i % TotalSectorsPerLap;
                if (sectorNumberOnLap == 0)
                {
                    driver.CurrentLapTime = 0;
                    driver.CurrentLap++;
                    _currentLap = Math.Max(driver.CurrentLap, _currentLap);
                }

                var startPositionPenalty = i == 0 ? (driver.StartPosition - 1) * 85 : 0;
                var sectorTime = _sectors[sectorNumberOnLap] switch
                {
                    SectorType.Cornering => Randomizer.GetRandomNumberBetween(idealSectorTime, maxCorneringSectorTime),
                    SectorType.Acceleration => Randomizer.GetRandomNumberBetween(idealSectorTime,
                        maxAccelerationSectorTime),
                    SectorType.Breaking => Randomizer.GetRandomNumberBetween(idealSectorTime, maxBreakingSectorTime),
                    _ => idealSectorTime
                };
                sectorTime += startPositionPenalty;

                await Task.Delay(sectorTime);

                driver.TotalSectorsPassed++;
                driver.TotalTime += sectorTime;
                driver.CurrentLapTime += sectorTime;
                driver.TimesPerSector[i] = driver.TotalTime;

                lock (_orderingLocker)
                {
                    _drivers = _drivers.OrderByDescending(d =>
                        d.TotalSectorsPassed == 0
                            ? 0
                            : d.TotalSectorsPassed * 10000000000 - d.TimesPerSector[d.TotalSectorsPassed - 1]).ToList();
                }

                if (sectorNumberOnLap == TotalSectorsPerLap - 1)
                {
                    driver.BestLap = Math.Min(driver.BestLap, driver.CurrentLapTime);
                    lock (_bestLapLocker)
                    {
                        _bestLap = Math.Min(_bestLap, driver.CurrentLapTime);
                        if (_bestLap == driver.CurrentLapTime)
                            _bestLapHolder = driver.DriverModel.Name;
                    }
                }
            }

            driver.IsFinished = true;
            if (_drivers.All(d => d.IsFinished))
            {
                MakeResult();
            }
        }

        private string MakeSnapshot()
        {
            var sb = new StringBuilder($"Трасса {_currentTrack.Name}\nТекущий круг: {_currentLap} / {TotalLaps}\n");

            var driversInfo = _drivers.Select((driver, i) =>
            {
                var pos = i + 1;
                var ebalo = driver.UsableEmoji;

                var lap = driver.CurrentLap;

                var result = $"{pos}.\t{ebalo}";

                if (lap != 0 && driver.TotalSectorsPassed != 0)
                {
                    var gapToLeader = Math.Abs(driver.TimesPerSector[driver.TotalSectorsPassed - 1] -
                                               _drivers[0].TimesPerSector[driver.TotalSectorsPassed - 1]);

                    result += $"\tКРУГ {lap}\t+{Utility.NormalizeTime(gapToLeader)}";
                }

                if (driver.IsUser) result += $"\t - {driver.Username}";

                return result;
            });

            sb.Append(string.Join("\n", driversInfo));

            if (_currentLap > 1)
            {
                sb.Append('\n')
                    .Append($"ЛУЧШИЙ КРУГ: {Utility.NormalizeTime(_bestLap)} от {_bestLapHolder}");
            }

            return sb.ToString();
        }

        private void MakeResult()
        {
            _isFinished = true;

            var botPosition = _drivers.Count;

            var sb = new StringBuilder($"РЕЗУЛЬТАТЫ ГОНОЧКИ В {_currentTrack.Name}\n");
            var driversInfo = _drivers.Select((d, i) =>
            {
                var pos = i + 1;
                if (d.DiscordId == Constants.BotId) botPosition = pos;

                var result = $"{pos}.\t{d.UsableEmoji}";
                if (!d.IsUser) return result;

                var user = Configuration.GetServerMember(d.DiscordId);
                result += $"\t{user.ServerOrUserName()}";

                if (pos < botPosition && pos <= 10)
                {
                    var pts = Points[i];
                    _config.ChangeBalance(d.DiscordId, pts, $"{pos} место в гонке");
                    result += $"\t+{pts} scam coins";
                }

                var diff = Math.Abs(d.StartPosition - pos);
                for (var j = 0; j < diff; j++)
                {
                    switch (Randomizer.GetRandomNumberBetween(0, 3))
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

            sb.Append(string.Join("\n", driversInfo));
            _mainRaceMessage.ModifyAsync(sb.ToString());

            var models = _drivers.Select(d => d.DriverModel).ToList();
            File.WriteAllText(@"drivers.json", JsonConvert.SerializeObject(models, Formatting.Indented));

            _config.CurrentRace = null;
        }
    }
}