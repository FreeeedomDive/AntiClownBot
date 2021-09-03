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

        public bool IsCooldownPassed()
        {
            return DateTime.Now > Economy.NextTribute;
        }
    }
}