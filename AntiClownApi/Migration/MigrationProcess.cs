using System;
using System.Linq;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBControllers;

namespace AntiClownBotApi.Migration
{
    public class MigrationProcess
    {
        private DatabaseContext Database { get; }
        private UserRepository UserRepository { get; }

        public MigrationProcess(DatabaseContext database, UserRepository userRepository)
        {
            Database = database;
            UserRepository = userRepository;
        }
        
        public void StartMigration()
        {
            var oldConfig = Configuration.GetConfiguration();
            foreach (var (id, user) in oldConfig.Users)
            {
                var newUser = UserRepository.CreateNewUserWithDbConnection(id);
                var balance = user.SocialRating +
                                            (int) user
                                                .Items
                                                .Select((item) => Math.Max(0, item.Key.Price * 1.2) * item.Value)
                                                .Sum();
                newUser.Economy.ScamCoins = balance;
                Console.WriteLine($"{user.DiscordUsername} has {balance}");
            }

            Database.SaveChanges();
            UserRepository.Save();
        }
    }
}