using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using Microsoft.EntityFrameworkCore;

namespace AntiClownBotApi.Database.DBControllers
{
    public class NewUserRepository
    {
        private DatabaseContext Database { get; }

        public NewUserRepository(DatabaseContext database)
        {
            Database = database;
        }

        public bool IsUserExist(ulong id)
        {
            return Database.Users.Any(user => user.DiscordId == id);
        }

        public DbUser CreateNewUser(ulong id)
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
            Database.Users.Add(user);
            Write();
            return user;
        }

        public DbUser ReadUser(ulong id)
        {
            var user = IsUserExist(id)
                ? Database.Users
                    .Include(u => u.Economy)
                    .Include(u => u.Items)
                    .Include(u => u.Stats)
                    .Include(u => u.Shop)
                    .First(u => u.DiscordId == id)
                : CreateNewUser(id);
            return user;
        }

        public void Write()
        {
            Database.SaveChanges();
        }
    }
}