using AntiClownBot.Models.User.Inventory;
using AntiClownBot.Models.User.Inventory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiClownBot
{
    public enum InventoryItem
    {
        CatWife,
        DogWife,
        RiceBowl,
        Gigabyte,
        JadeRod,
        CommunismPoster,
        None
    }

    public class SocialRatingUser
    {
        private static readonly List<InventoryItem> AllItems =
            Enum.GetValues(typeof(InventoryItem)).Cast<InventoryItem>().Where(item => item != InventoryItem.None)
                .ToList();

        public ulong DiscordId;
        public string DiscordUsername;
        public int SocialRating;
        public readonly Dictionary<Item, int> Items;
        public int NetWorth => SocialRating + Items.Keys
            .Where(item =>
                item == new DogWife() ||
                item == new CatWife() ||
                item == new Gigabyte() ||
                item == new RiceBowl())
            .Select(key => Items[key] * 1000).Sum();

        public readonly Dictionary<InventoryItem, int> UserItems;
        public DateTime NextTribute;

        public SocialRatingUser()
        {
            Items = new Dictionary<Item, int>
            {
                {new CatWife(), UserItems[InventoryItem.CatWife] },
                {new DogWife(), UserItems[InventoryItem.DogWife] },
                {new RiceBowl(), UserItems[InventoryItem.RiceBowl] },
                {new Gigabyte(), UserItems[InventoryItem.Gigabyte] },
                {new JadeRod(), UserItems[InventoryItem.JadeRod] },
                {new CommunismPoster(), UserItems[InventoryItem.CommunismPoster] },
                {new LootBox(), 0 }
            };
        }
        public SocialRatingUser(ulong id, string name)
        {
            DiscordId = id;
            DiscordUsername = name;
            SocialRating = Constants.DefaultSocialRating;
            NextTribute = DateTime.MinValue;
            UserItems = new Dictionary<InventoryItem, int>
            {
                {InventoryItem.CatWife, 0},
                {InventoryItem.DogWife, 0},
                {InventoryItem.RiceBowl, 0},
                {InventoryItem.Gigabyte, 0},
                {InventoryItem.JadeRod, 0},
                {InventoryItem.CommunismPoster, 0}
            };
        }

        public void ChangeRating(int rating)
        {
            SocialRating += rating;
            var config = Configuration.GetConfiguration();
            config.DailyStatistics.CreditsCollected += rating;
            config.DailyStatistics.ChangeUserCredits(DiscordUsername, rating);
            Configuration.GetConfiguration().Save();
        }

        public string LoseRandomItems(int count)
        {
            var stringBuilder = new StringBuilder();
            var config = Configuration.GetConfiguration();
            while (count > 0)
            {
                if (!Items.Any(item => item.Value > 0))
                {
                    stringBuilder.Append($"У {DiscordUsername} нет предметов, удалять нечего");
                    Configuration.GetConfiguration().Save();
                    return stringBuilder.ToString();
                }

                var item = Items.Where(item => item.Value > 0).SelectRandomItem().Key;
                Items[item]--;
                config.DailyStatistics.CreditsCollected -= item.Price;
                config.DailyStatistics.ChangeUserCredits(DiscordUsername, -item.Price);
                stringBuilder.Append($"{DiscordUsername} теряет {item.Name}\n");
                count--;
            }

            config.Save();
            return stringBuilder.ToString();
        }

        public string AddRandomItems(int count)
        {
            var stringBuilder = new StringBuilder();
            var config = Configuration.GetConfiguration();
            while (count > 0)
            {
                var item = Items.SelectRandomItem().Key;
                Items[item]++;
                config.DailyStatistics.CreditsCollected += item.Price;
                config.DailyStatistics.ChangeUserCredits(DiscordUsername, item.Price);
                stringBuilder.Append($"{DiscordUsername} получает {item.Name}\n");
                count--;
            }

            config.Save();
            return stringBuilder.ToString();
        }

        public void AddCustomItem(Item item)
        {
            Items[item]++;
            var config = Configuration.GetConfiguration();
            config.DailyStatistics.CreditsCollected += item.Price;
            config.DailyStatistics.ChangeUserCredits(DiscordUsername, item.Price);
            config.Save();
        }

        public void RemoveCustomItem(Item item)
        {
            if (Items[item] <= 0) return;

            Items[item]--;
            var config = Configuration.GetConfiguration();
            config.DailyStatistics.CreditsCollected -= item.Price;
            config.DailyStatistics.ChangeUserCredits(DiscordUsername, -item.Price);
            config.Save();
        }

        public bool HasDodgedPidor()
        {
            return Randomizer.GetRandomNumberBetween(0, 100) <
                   Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForDogWife,
                       Items[new DogWife()]);
        }

        public (int, int) UpdateCooldown()
        {
            var cooldown = Constants.DefaultCooldown;

            var gigabyteWorked = 0;
            var jadeRodWorked = 0;

            for (var i = 0; i < Items[new Gigabyte()]; i++)
            {
                if (!(Randomizer.GetRandomNumberBetween(0, 100) <
                      Constants.CooldownDecreaseChanceByOneGigabyte)) continue;
                gigabyteWorked++;
                cooldown *= 1 - Constants.CooldownDecreaseByOneGigabyteItem;
            }

            for (var i = 0; i < Items[new JadeRod()]; i++)
            {
                if (!(Randomizer.GetRandomNumberBetween(0, 100) < Constants.CooldownIncreaseChanceByOneJade)) continue;
                jadeRodWorked++;
                cooldown *= Constants.CooldownIncreaseByOneJade;
            }

            NextTribute = DateTime.Now.AddMilliseconds(cooldown);
            return (gigabyteWorked, jadeRodWorked);
        }

        public bool IsCooldownPassed()
        {
            return DateTime.Now > NextTribute;
        }

        public override string ToString()
        {
            return $"{DiscordUsername} {SocialRating}";
        }
    }
}