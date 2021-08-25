using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models.Items;
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
                Transactions = new List<DbTransaction>(),
                LootBoxes = 0
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

        public static DbUser GetUserWithEconomyAndItems(ulong id)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(id)
                ? database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Items)
                    .ThenInclude(i => i.ItemStats)
                    .First(u => u.DiscordId == id)
                : CreateNewUserWithDbConnection(id, database);
            return user;
        }

        public static List<ulong> GetAllUsersIds()
        {
            using var database = new DatabaseContext();
            return database.Users.Select(user => user.DiscordId).ToList();
        }

        public static List<DbUser> GetAllUsersWithEconomyAndItems()
        {
            using var database = new DatabaseContext();
            return database.Users
                .Include(u => u.Economy)
                .Include(u => u.Items)
                .ToList();
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
                ? database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Economy.Transactions)
                    .First(u => u.DiscordId == id)
                : CreateNewUserWithDbConnection(id, database);
            user.Economy.ScamCoins += ratingDiff;
            var transaction = new DbTransaction()
            {
                UserEconomy = user.Economy,
                DateTime = DateTime.Now,
                RatingChange = ratingDiff,
                Description = reason
            };
            user.Economy.Transactions.Add(transaction);
            database.SaveChanges();
        }

        public static void UpdateUserTributeCooldown(ulong userId, int cooldown)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(userId)
                ? database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == userId)
                : CreateNewUserWithDbConnection(userId, database);
            user.Economy.NextTribute = DateTime.Now.AddMilliseconds(cooldown);
            database.SaveChanges();
        }

        public static DateTime GetUserNextTributeDateTime(ulong userId)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(userId)
                ? database.Users
                    .Include(u => u.Economy)
                    .First(u => u.DiscordId == userId)
                : CreateNewUserWithDbConnection(userId, database);
            return user.Economy.NextTribute;
        }

        public static void RemoveCooldowns()
        {
            using var database = new DatabaseContext();
            database.Users
                .Include(u => u.Economy)
                .ForEach(user => user.Economy.NextTribute = DateTime.Now);
            database.SaveChanges();
        }

        public static ulong GetRichestUser()
        {
            using var database = new DatabaseContext();
            return database.Users
                .Include(u => u.Economy)
                .OrderByDescending(u => u.Economy.ScamCoins).First().DiscordId;
        }

        public static ItemResult GetItemById(ulong userId, Guid itemId, out BaseItem item)
        {
            item = null;
            var database = new DatabaseContext();
            var user = IsUserExist(userId)
                ? database.Users
                    .Include(u => u.Items)
                    .ThenInclude(i => i.ItemStats)
                    .First(u => u.DiscordId == userId)
                : CreateNewUserWithDbConnection(userId, database);

            var items = user.Items.Where(i => i.Id == itemId).ToList();
            if (!items.Any()) 
                return ItemResult.ItemNotFound;
            
            item = BaseItem.FromDbItem(items[0]);
            return ItemResult.Success;
        }
    }
}