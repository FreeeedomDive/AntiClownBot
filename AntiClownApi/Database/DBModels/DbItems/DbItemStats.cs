using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiClownBotApi.Database.DBModels.DbItems
{
    public class DbItemStats
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        // foreign key
        public Guid ItemId { get; set; }
        public DbItem Item { get; set; }
        
        public int CatAutoTributeChance { get; set; }
        public int CommunismDivideChance { get; set; }
        public int CommunismStealChance { get; set; }
        public int DogLootBoxFindChance { get; set; }
        public int InternetSpeed { get; set; }
        public int InternetGigabytes { get; set; }
        public int InternetPing { get; set; }
        public int JadeRodThickness { get; set; }
        public int JadeRodLength { get; set; }
        public int RiceNegativeRangeExtend { get; set; }
        public int RicePositiveRangeExtend { get; set; }
    }
}