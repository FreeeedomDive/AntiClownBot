using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        // foreign key
        public ulong UserEconomyId { get; set; }
        public DbUserEconomy UserEconomy { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public int RatingChange { get; set; }
        public string Description { get; set; }
    }
}