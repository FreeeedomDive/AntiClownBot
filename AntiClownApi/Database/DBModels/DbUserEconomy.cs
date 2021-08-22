using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUserEconomy
    {
        // foreign key
        [Key]
        public ulong UserId { get; set; }
        public DbUser User { get; set; }

        public int ScamCoins { get; set; }
        public DateTime NextTribute { get; set; }
        public List<DbTransaction> Transactions { get; set; }
    }
}