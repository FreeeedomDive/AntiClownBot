using AntiClownBot.Models.User.Inventory;
using AntiClownBot.Models.User.Inventory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntiClownBot.Models.User.Stats;

namespace AntiClownBot
{
    public class SocialRatingUser
    {
        public ulong DiscordId;
        public string DiscordUsername;
        public int SocialRating;
        public Dictionary<Item, int> Items;
        public UserStats Stats;

        public int NetWorth => SocialRating + Items.Keys
            .Where(item =>
                item.Equals(new DogWife()) ||
                item.Equals(new CatWife()) ||
                item.Equals(new Gigabyte()) ||
                item.Equals(new RiceBowl()) ||
                item.Equals(new LootBox()))
            .Select(key => Items[key] * key.Price).Sum();

        public DateTime NextTribute;

        public SocialRatingUser(ulong id, string name)
        {
            DiscordId = id;
            DiscordUsername = name;
            SocialRating = Constants.DefaultSocialRating;
            NextTribute = DateTime.MinValue;
            Items = AllItems.GetAllItems().ToDictionary(item => item, _ => 0);
            Stats = new UserStats();
        }

        public void ChangeRating(int rating)
        {
            SocialRating += rating;
            Transactions.TransactionLog.AddLog(this, $"изменение рейтинга на {rating}");
            var config = Configuration.GetConfiguration();
            config.DailyStatistics.CreditsCollected += rating;
            config.DailyStatistics.ChangeUserCredits(DiscordUsername, rating);
            Configuration.GetConfiguration().Save();
        }

        public void AddCustomItem(Item item)
        {
            Items[item]++;
            var config = Configuration.GetConfiguration();
            if (item.Price > 0)
            {
                config.DailyStatistics.CreditsCollected += item.Price;
                config.DailyStatistics.ChangeUserCredits(DiscordUsername, item.Price);
            }
            item.OnItemAddOrRemove(this);
            config.Save();
        }

        public void RemoveCustomItem(Item item)
        {
            if (Items[item] <= 0) return;

            Items[item]--;
            var config = Configuration.GetConfiguration();
            if (item.Price > 0)
            {
                config.DailyStatistics.CreditsCollected -= item.Price;
                config.DailyStatistics.ChangeUserCredits(DiscordUsername, -item.Price);
            }
            item.OnItemAddOrRemove(this);
            config.Save();
        }

        public string Use(Item item)
        {
            if (Items[item] < 1)
            {
                return $"{DiscordUsername} не иметь {item.Name}";
            }

            RemoveCustomItem(item);
            return item.Use(this);
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

            var gigabyteCount = Items[new Gigabyte()];
            for (var i = 0; i < gigabyteCount; i++)
            {
                if (!(Randomizer.GetRandomNumberBetween(0, 100) <
                      Constants.CooldownDecreaseChanceByOneGigabyte)) continue;
                gigabyteWorked++;
                cooldown *= 1 - Constants.CooldownDecreaseByOneGigabyteItem;
            }

            var jadeRodCount = Items[new JadeRod()];
            for (var i = 0; i < jadeRodCount; i++)
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