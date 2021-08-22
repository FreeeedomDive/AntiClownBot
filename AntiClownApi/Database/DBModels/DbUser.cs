using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUser
    {
        [Key] public ulong DiscordId { get; set; }
        public DbUserEconomy Economy { get; set; }
        public List<DbItem> Items { get; set; }
        public DbUserShop Shop { get; set; }
        public DbUserStats Stats { get; set; }

        public Dictionary<int, int> UpdateCooldown()
        {
            var result = new Dictionary<int, int>();

            var cooldown = Items
                .Where(item => item.Name.Equals(StringConstants.InternetName))
                .SelectMany(item => Enumerable.Repeat(item, item.ItemStats.InternetGigabytes))
                .Aggregate(NumericConstants.DefaultCooldown, (currentCooldown, item) =>
                    currentCooldown * Randomizer.GetRandomNumberBetween(0, 100) < item.ItemStats.InternetPing
                        ? (100d - item.ItemStats.InternetSpeed) / 100
                        : 1);
            
            cooldown = Items
                .Where(item => item.Name.Equals(StringConstants.JadeRodName))
                .Aggregate(cooldown, (currentCooldown, item) =>
                    currentCooldown * Randomizer.GetRandomNumberBetween(0, 100) < item.ItemStats.JadeRodLength
                        ? (100d + item.ItemStats.JadeRodThickness) / 100
                        : 1);

            Economy.NextTribute = DateTime.Now.AddMilliseconds(cooldown);
            return result;
        }
    }
}