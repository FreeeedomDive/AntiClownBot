using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUser
    {
        [Key] public ulong DiscordId { get; set; }
        public DbUserEconomy Economy { get; set; }
        public List<DbItem> Items { get; set; }
        public DbUserShop Shop { get; set; }
        public DbUserStats Stats { get; set; }

        public Dictionary<Guid, int> UpdateCooldown()
        {
            var result = new Dictionary<Guid, int>();

            var cooldown = Items
                .Where(item => item.Name.Equals(StringConstants.InternetName))
                .SelectMany(item => Enumerable.Repeat(item, item.ItemStats.InternetGigabytes))
                .Aggregate(NumericConstants.DefaultCooldown, (currentCooldown, dbItem) =>
                {
                    var item = (Internet) dbItem;
                    
                    if (Randomizer.GetRandomNumberBetween(0, 100) >= item.Ping)
                        return currentCooldown;

                    if (result.ContainsKey(item.Id))
                    {
                        result[item.Id]++;
                    }
                    else
                    {
                        result.Add(item.Id, 1);
                    }

                    return currentCooldown * (100d - item.Speed) / 100;
                });
            
            cooldown = Items
                .Where(item => item.Name.Equals(StringConstants.JadeRodName))
                .SelectMany(item => Enumerable.Repeat(item, item.ItemStats.JadeRodLength))
                .Aggregate(cooldown, (currentCooldown, dbItem) =>
                {
                    var item = (JadeRod) dbItem;
                    
                    if (Randomizer.GetRandomNumberBetween(0, 100) >= NumericConstants.CooldownIncreaseChanceByOneJade)
                        return currentCooldown;

                    if (result.ContainsKey(item.Id))
                    {
                        result[item.Id]++;
                    }
                    else
                    {
                        result.Add(item.Id, 1);
                    }

                    return currentCooldown * (100d + dbItem.ItemStats.JadeRodThickness) / 100;
                });
            
            UserDbController.UpdateUserTributeCooldown(DiscordId, (int)cooldown);
            return result;
        }
        
        public bool IsCooldownPassed()
        {
            return DateTime.Now > Economy.NextTribute;
        }
    }
}