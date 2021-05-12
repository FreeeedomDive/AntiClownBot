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
        public readonly Dictionary<InventoryItem, int> UserItems;
        public DateTime NextTribute;

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
            Configuration.GetConfiguration().Save();
        }

        public string LoseRandomItems(int count)
        {
            var stringBuilder = new StringBuilder();
            while (count > 0)
            {
                if (!UserItems.Any(item => item.Value > 0))
                {
                    stringBuilder.Append($"У {DiscordUsername} нет предметов, удалять нечего");
                    Configuration.GetConfiguration().Save();
                    return stringBuilder.ToString();
                }

                var item = UserItems.Where(item => item.Value > 0).SelectRandomItem().Key;
                UserItems[item]--;
                stringBuilder.Append($"{DiscordUsername} теряет {Utility.ItemToString(item)}\n");
                count--;
            }

            Configuration.GetConfiguration().Save();
            return stringBuilder.ToString();
        }

        public string AddRandomItems(int count)
        {
            var stringBuilder = new StringBuilder();
            while (count > 0)
            {
                var item = AllItems.SelectRandomItem();
                UserItems[item]++;
                stringBuilder.Append($"{DiscordUsername} получает {Utility.ItemToString(item)}\n");
                count--;
            }

            Configuration.GetConfiguration().Save();
            return stringBuilder.ToString();
        }

        public void AddCustomItem(InventoryItem item)
        {
            UserItems[item]++;
            Configuration.GetConfiguration().Save();
        }

        public void RemoveCustomItem(InventoryItem item)
        {
            if (!UserItems.ContainsKey(item) || UserItems[item] <= 0) return;
            
            UserItems[item]--;
            Configuration.GetConfiguration().Save();
        }

        public bool HasDodgedPidor()
        {
            return Randomizer.GetRandomNumberBetween(0, 100) <
                   Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForDogWife,
                       UserItems[InventoryItem.DogWife]);
        }

        public (int, int) UpdateCooldown()
        {
            var cooldown = Constants.DefaultCooldown;

            var gigabyteWorked = 0;
            var jadeRodWorked = 0;

            for (var i = 0; i < UserItems[InventoryItem.Gigabyte]; i++)
            {
                if (!(Randomizer.GetRandomNumberBetween(0, 100) <
                      Constants.CooldownDecreaseChanceByOneGigabyte)) continue;
                gigabyteWorked++;
                cooldown *= 1 - Constants.CooldownDecreaseByOneGigabyteItem;
            }

            for (var i = 0; i < UserItems[InventoryItem.JadeRod]; i++)
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