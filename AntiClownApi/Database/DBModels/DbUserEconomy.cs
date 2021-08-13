using System;
using System.ComponentModel.DataAnnotations;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUserEconomy
    {
        [Key]
        public Guid Id { get; set; }
        
        // foreign key
        public ulong UserId { get; set; }
        public DbUser User { get; set; }

        public int SocialRating { get; set; }
        public DateTime NextTribute { get; set; }
    }
}