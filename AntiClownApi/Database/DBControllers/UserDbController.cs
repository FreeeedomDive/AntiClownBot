using System;
using System.Collections.Generic;
using System.Linq;
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
            return database.Users.Any(user => user.Id == id);
        }

        public static DbUser CreateNewUser(ulong id)
        {
            using var database = new DatabaseContext();
            var user = new DbUser()
            {
                Id = id,
                Items = new List<DbItem>(),
            };
            user.Economy = new DbUserEconomy()
            {
                Id = Guid.NewGuid(),
                User = user,
                NextTribute = DateTime.Now,
                SocialRating = 1000,
            };
            database.Users.Add(user);
            database.SaveChanges();
            return user;
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
                    .First(u => u.Id == id)
                : CreateNewUser(id);
            return user;
        }

        public static DbUser GetUserEconomy(ulong id)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(id)
                ? database.Users
                    .Include(u => u.Economy)
                    .First(u => u.Id == id)
                : CreateNewUser(id);
            return user;
        }

        public static void ChangeUserRating(ulong id, int ratingDiff, string reason)
        {
            using var database = new DatabaseContext();
            var user = IsUserExist(id)
                ? database.Users.Include(u => u.Economy).First(u => u.Id == id)
                : CreateNewUser(id);
            user.Economy.SocialRating += ratingDiff;
            database.SaveChanges();
        }
    }
}