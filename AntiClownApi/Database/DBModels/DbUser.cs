using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUser
    {
        [Key]
        public ulong Id { get; set; }
        
        public DbUserEconomy Economy { get; set; }
        public List<DbItem> Items { get; set; }
        public DbUserShop Shop { get; set; }
        public DbUserStats Stats { get; set; }
    }
}