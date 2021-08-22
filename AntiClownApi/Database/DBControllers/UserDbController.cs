using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using Microsoft.EntityFrameworkCore;

namespace AntiClownBotApi.Database.DBControllers
{
    public static class UserDbController
    {
        public static bool IsUserExist(ulong id)
        {
            using var database = new DatabaseContext();
            return database.Users.Any(user => user.DiscordId == id);
        }

        public static DbUser CreateNewUserWithDbConnection(ulong id, DatabaseContext database)
        {
            var user = new DbUser()
            {
                DiscordId = id,
                Items = new List<DbItem>(),
                Shop = DbUserShop.GenerateNewShopForUser(id),
                Stats = new DbUserStats()
            };
            user.Economy = new DbUserEconomy()
            {
                User = user,
                NextTribute = DateTime.Now,
                ScamCoins = NumericConstants.DefaultScamCoins,
                Transactions = new List<DbTransaction>()
            };
            database.Users.Add(user);
            database.SaveChanges();
            return user;
        }

        public static DbUser CreateNewUser(ulong id)
        {
            using var database = new DatabaseContext();
            return CreateNewUserWithDbConnection(id, database);
        }

        public static DbUser GetFullUser(ulong id)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(id)
                ? database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Items)
                    .Include(u => u.Stats)
                    .Include(u => u.Shop)
                    .First(u => u.DiscordId == id)
                : CreateNewUserWithDbConnection(id, database);
            return user;
        }

        public static DbUser GetUserEconomy(ulong id)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(id)
                ? database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == id)
                : CreateNewUserWithDbConnection(id, database);
            return user;
        }

        public static void ChangeUserBalance(ulong id, int ratingDiff, string reason)
        {
            using var database = new DatabaseContext();
            ChangeUserBalanceWithConnection(id, ratingDiff, reason, database);
        }

        public static void ChangeUserBalanceWithConnection(ulong id, int ratingDiff, string reason,
            DatabaseContext database)
        {
            var user = IsUserExist(id)
                ? database.Users.Include(u => u.Economy)
                    .First(u => u.DiscordId == id)
                : CreateNewUserWithDbConnection(id, database);
            user.Economy.ScamCoins += ratingDiff;
            user.Economy.Transactions.Add(new DbTransaction()
            {
                UserEconomy = user.Economy,
                RatingChange = ratingDiff,
                Description = reason
            });
            database.SaveChanges();
        }
    }
}