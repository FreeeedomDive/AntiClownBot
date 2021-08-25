using System;
using System.Linq;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBControllers;

namespace AntiClownBotApi.Migration
{
    public class MigrationProcess
    {
        public static void StartMigration()
        {
            var database = new DatabaseContext();
            var oldConfig = Configuration.GetConfiguration();
            foreach (var (id, user) in oldConfig.Users)
            {
                var newUser = UserDbController.CreateNewUserWithDbConnection(id, database);
                var balance = user.SocialRating +
                                            (int) user
                                                .Items
                                                .Select((item) => Math.Max(0, item.Key.Price * 1.2) * item.Value)
                                                .Sum();
                newUser.Economy.ScamCoins = balance;
                Console.WriteLine($"{user.DiscordUsername} has {balance}");
            }

            database.SaveChanges();
        }
    }
}