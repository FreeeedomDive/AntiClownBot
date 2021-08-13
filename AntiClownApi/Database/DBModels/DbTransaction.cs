using System.ComponentModel.DataAnnotations;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbTransaction
    {
        [Key]
        public int Id { get; set; }
        
        // foreign key
        public ulong DbUserId { get; set; }
        
        public int RatingChange { get; set; }
        public string Description { get; set; }
    }
}