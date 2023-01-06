using AntiClownDiscordBotVersion2.Utils;
using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Models.Lohotron
{
    public class Lohotron
    {
        public Lohotron(IRandomizer randomizer)
        {
            var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");

            fileName = $"{filesDirectory}/StatisticsFiles/lohotron.json";
            this.randomizer = randomizer;
            UsersId = CreateOrRestore();
        }

        public void Reset()
        {
            UsersId.Clear();
            Save();
        }

        private static readonly ILohotronPrize[] AllPrizes =
        {
            new DefaultLohotronPrize(),
            new CreditsLohotronPrize(1),
            new CreditsLohotronPrize(100),
            new CreditsLohotronPrize(200),
            new CreditsLohotronPrize(250),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(500),
            new CreditsLohotronPrize(-500),
            new LootBoxLohotronPrize()
        };

        private static readonly Wheel Wheel =
            new(
                new(40, AllPrizes[0]), // Nothing
                new(30, AllPrizes[1]), // +1
                new(14, AllPrizes[2]), // +100
                new(7, AllPrizes[3]), // +200
                new(1, AllPrizes[4]), // +250
                new(1, AllPrizes[5]), // +500
                new(1, AllPrizes[6]), // +500
                new(1, AllPrizes[7]), // +500
                new(3, AllPrizes[8]), // -500
                new(6, AllPrizes[9]) // Lootbox
            );

        public ILohotronPrize Play(ulong userId)
        {
            UsersId.Add(userId);
            Save();
            return Wheel.GetPrize(randomizer.GetRandomNumberBetween(0, Wheel.PrizesCount));
        }

        private List<ulong> CreateOrRestore()
        {
            if (!File.Exists(fileName))
            {
                return new List<ulong>();
            }

            var content = JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText(fileName));
            return content ?? new List<ulong>();
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(UsersId, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public readonly List<ulong> UsersId;

        private readonly IRandomizer randomizer;
        private static string fileName = "";
    }
}